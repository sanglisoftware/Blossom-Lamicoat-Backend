using Api.Application.DTOs;
namespace Api.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> AuthenticateAsync(AuthRequestDto request);

    Task<bool> ChangeExistingPassword(string oldPassword, string newPassword);
}
