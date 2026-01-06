using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class RoleRepository(AppDbContext _context) : IRoleRepository
{

    public async Task<IEnumerable<Role>> GetAllAsync() => await _context.Roles.ToListAsync();

    public async Task<Role?> GetByIdAsync(int id) => await _context.Roles.FindAsync(id);

    public async Task<Role> CreateAsync(Role Role)
    {
        _context.Roles.Add(Role);
        await _context.SaveChangesAsync();
        return Role;
    }

    public async Task<Role?> UpdateAsync(int id, Role Role)
    {
        var existing = await _context.Roles.FindAsync(id);
        if (existing == null) return null;

        existing.RoleValue = Role.RoleValue;
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var Role = await _context.Roles.FindAsync(id);
        if (Role == null) return false;

        _context.Roles.Remove(Role);
        await _context.SaveChangesAsync();
        return true;
    }

    public IQueryable<Role> Query() => _context.Roles.AsNoTracking();
}
