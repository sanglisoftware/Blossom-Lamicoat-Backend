using Api.Domain.Entities;
namespace Api.Infrastructure.Repositories;

public interface IAuthRepository
{
    Task<User?> GetUserAsync(string username, string password);
    Task UpdateLoginStatusAsync(string username);

    Task<bool> ChangePassword(string oldPassword, string newPassword);
}
