using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class ColourRepository(AppDbContext _context) : IColourRepository
{
    public async Task<IEnumerable<Colour>> GetAllAsync() => await _context.Colour.ToListAsync();

    public async Task<Colour?> GetByIdAsync(int id) => await _context.Colour.FindAsync(id);

    public async Task<Colour> AddAsync(Colour colour)
    {
        await _context.Colour.AddAsync(colour);
        return colour;
    }

    public async Task<Colour?> UpdateAsync(int id, Colour colour)
    {
        var existing = await _context.Colour.FindAsync(id);
        if (existing == null) return null;

        _context.Entry(existing).CurrentValues.SetValues(colour);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var colour = await _context.Colour.FindAsync(id);
        if (colour == null)
            return false;

        _context.Colour.Remove(colour);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Colour?> GetByNameAsync(string name)
    {
        return await _context.Colour.FirstOrDefaultAsync(e => e.Name == name);
    }

    public IQueryable<Colour> Query() =>
        _context.Colour
            .Select(x => new Colour
            {
                Id = x.Id, // Add if needed
                Name = x.Name,
                IsActive = x.IsActive,
            });
}
