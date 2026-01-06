using Api.Application.DTOs;
using Api.Application.Interfaces;
using AutoMapper;
using Api.Domain.Entities;
using Api.Infrastructure.Repositories;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace Api.Application.Services;

using Microsoft.Extensions.Logging;


public class MenuService(IMenuRepository _menuRepository, ILogger<MenuService> _logger, AppDbContext _context) : IMenuService
{//IMapper _mapper, 
    public async Task<IEnumerable<MenuDto>> GetMenuForRoleAsync(int roleId)
    {
        _logger.LogInformation("Building menu for role {RoleId}", roleId);

        try
        {
            var allMenus = await _menuRepository.GetAllAsync();
            _logger.LogDebug("Retrieved {Count} menus from repository", allMenus.Count());

            var permissions = (await _menuRepository.GetPermissionsByRoleAsync(roleId))
                .ToDictionary(p => p.MenuId);
            _logger.LogDebug("Retrieved {Count} permissions for role {RoleId}", permissions.Count, roleId);

            // Create parent-child mapping
            var menuLookup = allMenus.ToLookup(m => m.ParentId);

            // Determine which menu IDs should be visible
            var visibleIds = new HashSet<int>();
            DetermineVisibleIds(null, menuLookup, permissions, visibleIds);

            // Build the tree only with visible items
            return BuildMenuTree(null, menuLookup, permissions, visibleIds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building menu for role {RoleId}", roleId);
            throw;
        }
    }

    private static void DetermineVisibleIds(int? parentId, ILookup<int?, Menu> menuLookup, Dictionary<int, RoleMenuPermission> permissions, HashSet<int> visibleIds)
    {
        foreach (var menu in menuLookup[parentId].OrderBy(m => m.Sequence))
        {
            // Leaf node: visible if has any permission
            if (!string.IsNullOrEmpty(menu.Pathname))
            {
                if (permissions.ContainsKey(menu.Id)) // ← key change here
                {
                    visibleIds.Add(menu.Id);
                }
            }
            else // Parent node
            {
                // Check children first
                DetermineVisibleIds(menu.Id, menuLookup, permissions, visibleIds);

                // Parent is visible if any child is visible
                if (visibleIds.Any(id =>
                    menuLookup[menu.Id].Any(child => child.Id == id)))
                {
                    visibleIds.Add(menu.Id);
                }
            }
        }
    }

    private List<MenuDto> BuildMenuTree(int? parentId, ILookup<int?, Menu> menuLookup, Dictionary<int, RoleMenuPermission> permissions, HashSet<int> visibleIds)
    {
        return [.. menuLookup[parentId]
            .Where(menu => visibleIds.Contains(menu.Id)) // Only include visible items
            .OrderBy(m => m.Sequence)
            .Select(menu =>
            {
                var dto = new MenuDto
                {
                    Id = menu.Id,
                    Icon = menu.Icon,
                    Pathname = menu.Pathname,
                    Title = menu.Title
                };

                // Handle leaf nodes
                if (!string.IsNullOrEmpty(menu.Pathname))
                {
                    if (permissions.TryGetValue(menu.Id, out var perm))
                    {
                        dto.Permission = new PermissionDto
                        {
                            Create = perm.CreatePermission,
                            Update = perm.UpdatePermission,
                            Delete = perm.DeletePermission
                        };
                    }
                    return dto;
                }

                // Handle parent nodes
                dto.SubMenu = BuildMenuTree(menu.Id, menuLookup, permissions, visibleIds);
                return dto;
            })];
    }

    public async Task<bool> SavePermissionsForRoleAsync(RoleMenuPermissionRequest request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Remove existing permissions in batches if large dataset
            var existing = await _context.RoleMenuPermissions
                .Where(p => p.RoleId == request.RoleId)
                .ToListAsync();

            if (existing.Any())
            {
                _context.RoleMenuPermissions.RemoveRange(existing);
                await _context.SaveChangesAsync(); // Save before adding new ones
            }

            // Create new permissions with explicit values
            var permissions = request.Permissions.Select(p => new RoleMenuPermission
            {
                RoleId = request.RoleId,
                MenuId = p.MenuId,
                CreatePermission = p.Create,
                UpdatePermission = p.Update,
                DeletePermission = p.Delete
            }).ToList();

            foreach (var p in permissions)
            {
                await _context.Database.ExecuteSqlInterpolatedAsync($@"
        MERGE role_menu_permission_transaction AS target
        USING (SELECT 
                {p.RoleId} AS role_id, 
                {p.MenuId} AS menu_id) AS source
        ON target.role_id = source.role_id 
           AND target.menu_id = source.menu_id

        WHEN MATCHED THEN
            UPDATE SET 
                create_permission = {p.CreatePermission},
                update_permission = {p.UpdatePermission},
                delete_permission = {p.DeletePermission}

        WHEN NOT MATCHED THEN
            INSERT (role_id, menu_id, create_permission, update_permission, delete_permission)
            VALUES ({p.RoleId}, {p.MenuId}, {p.CreatePermission}, {p.UpdatePermission}, {p.DeletePermission});
    ");
            }


            await transaction.CommitAsync();
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error saving permissions for role {RoleId}", request.RoleId);
            throw;
        }
    }

    public async Task<IEnumerable<MenuSelectionDto>> GetAllMenusWithSelectionAsync(int roleId)
    {
        _logger.LogInformation("Getting all menus with selection for role {RoleId}", roleId);

        try
        {
            var allMenus = await _menuRepository.GetAllAsync();
            var permissions = (await _menuRepository.GetPermissionsByRoleAsync(roleId))
                .ToDictionary(p => p.MenuId);

            // Create parent-child mapping
            var menuLookup = allMenus.ToLookup(m => m.ParentId);

            // Build the full tree with selection status
            return BuildSelectionTree(null, menuLookup, permissions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all menus with selection for role {RoleId}", roleId);
            throw;
        }
    }

    private List<MenuSelectionDto> BuildSelectionTree(
        int? parentId,
        ILookup<int?, Menu> menuLookup,
        Dictionary<int, RoleMenuPermission> permissions)
    {
        return menuLookup[parentId]
            .OrderBy(m => m.Sequence)
            .Select(menu =>
            {
                var isLeaf = !string.IsNullOrEmpty(menu.Pathname);
                var hasPermission = isLeaf && permissions.TryGetValue(menu.Id, out var perm);
                // && (perm.CreatePermission || perm.UpdatePermission || perm.DeletePermission);

                var dto = new MenuSelectionDto
                {
                    Id = menu.Id,
                    Icon = menu.Icon,
                    Pathname = menu.Pathname,
                    Title = menu.Title,
                    Selected = hasPermission // Set selection status
                };

                // Handle permissions for leaf nodes
                if (isLeaf && permissions.TryGetValue(menu.Id, out var permission))
                {
                    dto.Permission = new PermissionDto(
                        permission.CreatePermission,
                        permission.UpdatePermission,
                        permission.DeletePermission
                    );
                }

                // Recursively build submenus for non-leaf nodes
                if (!isLeaf)
                {
                    dto.SubMenu = BuildSelectionTree(menu.Id, menuLookup, permissions);
                }

                return dto;
            })
            .ToList();
    }

}