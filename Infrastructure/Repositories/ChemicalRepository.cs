using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class ChemicalRepository(AppDbContext _context) : IChemicalRepository
{
    public async Task<IEnumerable<Chemical>> GetAllAsync() => await _context.Chemical.ToListAsync();

    public async Task<Chemical?> GetByIdAsync(int id) => await _context.Chemical.FindAsync(id);

    public async Task<Chemical> AddAsync(Chemical chemical)
    {
        await _context.Chemical.AddAsync(chemical);
        return chemical;
    }

    public async Task<Chemical?> UpdateAsync(int id, Chemical chemical)
    {
        var existing = await _context.Chemical.FindAsync(id);
        if (existing == null) return null;

        _context.Entry(existing).CurrentValues.SetValues(chemical);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var chemical = await _context.Chemical.FindAsync(id);
        if (chemical == null)
            return false;

        _context.Chemical.Remove(chemical);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Chemical?> GetByNameAsync(string name)
    {
        return await _context.Chemical.FirstOrDefaultAsync(e => e.Name == name);
    }

    public IQueryable<Chemical> Query() =>
        _context.Chemical
            .Select(x => new Chemical
            {
                Id = x.Id, // Add if needed
                Name = x.Name,
                Type = x.Type,
                Comment = x.Comment,
                IsActive = x.IsActive,
            });
}
