using Api.Application.DTOs;
namespace Api.Application.Interfaces;

public interface IMenuService
{
    Task<IEnumerable<MenuDto>> GetMenuForRoleAsync(int roleId);

    Task<bool> SavePermissionsForRoleAsync(RoleMenuPermissionRequest request);

    Task<IEnumerable<MenuSelectionDto>> GetAllMenusWithSelectionAsync(int roleId); 

}