using Api.Domain.Entities;
namespace Api.Infrastructure.Repositories;

public interface IMenuRepository
{
    Task<IEnumerable<Menu>> GetAllAsync();
    Task<IEnumerable<RoleMenuPermission>> GetPermissionsByRoleAsync(int roleId);    
    Task SavePermissionsForRoleAsync(int roleId, IEnumerable<RoleMenuPermission> permissions);
}