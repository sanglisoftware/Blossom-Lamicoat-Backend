using Api.Domain.Entities;
namespace Api.Infrastructure.Repositories;

public interface IRoleRepository
{
    Task<IEnumerable<Role>> GetAllAsync();
    IQueryable<Role> Query();
    Task<Role?> GetByIdAsync(int id);
    Task<Role> CreateAsync(Role Role);
    Task<Role?> UpdateAsync(int id, Role Role);
    Task<bool> DeleteAsync(int id);
}
