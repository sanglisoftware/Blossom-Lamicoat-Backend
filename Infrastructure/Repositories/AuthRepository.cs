using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace Api.Infrastructure.Repositories;

public class AuthRepository(AppDbContext _context, IHttpContextAccessor _httpContextAccessor) : IAuthRepository
{
    public async Task<bool> ChangePassword(string oldPassword, string newPassword)
    {
        var username = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(username))
        {
            throw new UnauthorizedAccessException("User not authenticated.");
        }

        var user = await _context.Users.FindAsync(username) ?? throw new KeyNotFoundException("User not found.");

        // 3. Check if old password matches
        if (user.Password != oldPassword)
        {
            throw new ArgumentException("Old password is incorrect.");
        }

        // 4. Update password
        user.Password = newPassword;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<User?> GetUserAsync(string username, string password)
    {
        return await _context.Users
            .Where(u => u.Username == username && u.Password == password)
            .FirstOrDefaultAsync();
    }

    public async Task UpdateLoginStatusAsync(string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user != null)
        {
            user.LoginStatus = true;
            user.LastLoginDate = DateTime.Now;
            await _context.SaveChangesAsync();
        }
    }
}
