using Api.Infrastructure.Data;
using Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class SizeRepository(AppDbContext _context) : ISizeRepository
{
    public async Task<IEnumerable<Size>> GetAllAsync() => await _context.Sizes.ToListAsync();

    public async Task<Size?> GetByIdAsync(int id) => await _context.Sizes.FindAsync(id);

    public async Task<Size> CreateAsync(Size size)
    {
        _context.Sizes.Add(size);
        await _context.SaveChangesAsync();
        return size;
    }

    public async Task<Size?> UpdateAsync(int id, Size size)
    {
        var existing = await _context.Sizes.FindAsync(id);
        if (existing == null) return null;

        existing.SizeValue = size.SizeValue;
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var size = await _context.Sizes.FindAsync(id);
        if (size == null) return false;

        _context.Sizes.Remove(size);
        await _context.SaveChangesAsync();
        return true;
    }
}
