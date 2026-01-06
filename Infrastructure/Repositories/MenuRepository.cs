using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace Api.Infrastructure.Repositories;

public class MenuRepository(AppDbContext _context) : IMenuRepository
{
    public async Task<IEnumerable<Menu>> GetAllAsync()
    {
        return await _context.Menus.OrderBy(m => m.Sequence).ToListAsync();
    }

    public async Task<IEnumerable<RoleMenuPermission>> GetPermissionsByRoleAsync(int roleId)
    {
        return await _context.RoleMenuPermissions.Where(p => p.RoleId == roleId).ToListAsync();
    }

    public async Task SavePermissionsForRoleAsync(int roleId, IEnumerable<RoleMenuPermission> permissions)
    {
        // Remove existing permissions
        var existing = await _context.RoleMenuPermissions.Where(p => p.RoleId == roleId).ToListAsync();
        _context.RoleMenuPermissions.RemoveRange(existing);

        // Add new permissions
        await _context.RoleMenuPermissions.AddRangeAsync(permissions);
        await _context.SaveChangesAsync();
    }
}