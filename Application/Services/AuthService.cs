using Api.Application.DTOs;
using Api.Application.Interfaces;
using Api.Infrastructure.Repositories;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Api.Application.Services;

public class AuthService(
    IAuthRepository _repository,
    IConfiguration _config,
    IMenuService _menuService, // Injected menu service
    ILogger<AuthService> _logger) : IAuthService
{
    public async Task<AuthResponseDto?> AuthenticateAsync(AuthRequestDto request)
    {
        var user = await _repository.GetUserAsync(request.Username, request.Password);
        if (user == null) return null;

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddHours(1); // Session time

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        await _repository.UpdateLoginStatusAsync(user.Username);

        // Get dynamic menu for role
        IEnumerable<MenuDto> menu;
        try
        {
            menu = await _menuService.GetMenuForRoleAsync(user.Role);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading menu for role {RoleId}", user.Role);
            menu = [];
        }

        // Configure JSON serialization options
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };

        return new AuthResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = expires,
            MenuJson = JsonSerializer.Serialize(menu, jsonOptions)
        };
    }

    public Task<bool> ChangeExistingPassword(string oldPassword, string newPassword)
    {
        return _repository.ChangePassword(oldPassword, newPassword);
    }
}
