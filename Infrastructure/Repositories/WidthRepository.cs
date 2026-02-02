using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class WidthRepository(AppDbContext _context) : IWidthRepository
{
    public async Task<IEnumerable<Width>> GetAllAsync() => await _context.Width.ToListAsync();

    public async Task<Width?> GetByIdAsync(int id) => await _context.Width.FindAsync(id);

    public async Task<Width> AddAsync(Width width)
    {
        await _context.Width.AddAsync(width);
        return width;
    }

    public async Task<Width?> UpdateAsync(int id, Width width)
    {
        var existing = await _context.Width.FindAsync(id);
        if (existing == null) return null;

        _context.Entry(existing).CurrentValues.SetValues(width);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var width = await _context.Width.FindAsync(id);
        if (width == null)
            return false;

        _context.Width.Remove(width);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Width?> GetByNameAsync(string grm)
    {
        return await _context.Width.FirstOrDefaultAsync(e => e.GRM == grm);
    }

    public IQueryable<Width> Query() =>
        _context.Width
            .Select(x => new Width
            {
                Id = x.Id, // Add if needed
                GRM = x.GRM,
                IsActive = x.IsActive,
            });
}
