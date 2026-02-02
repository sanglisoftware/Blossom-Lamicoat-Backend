using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class GramageRepository(AppDbContext _context) : IGramageRepository
{
    public async Task<IEnumerable<Gramage>> GetAllAsync() => await _context.Gramage.ToListAsync();

    public async Task<Gramage?> GetByIdAsync(int id) => await _context.Gramage.FindAsync(id);

    public async Task<Gramage> AddAsync(Gramage gramage)
    {
        await _context.Gramage.AddAsync(gramage);
        return gramage;
    }

    public async Task<Gramage?> UpdateAsync(int id, Gramage gramage)
    {
        var existing = await _context.Gramage.FindAsync(id);
        if (existing == null) return null;

        _context.Entry(existing).CurrentValues.SetValues(gramage);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var gramage = await _context.Gramage.FindAsync(id);
        if (gramage == null)
            return false;

        _context.Gramage.Remove(gramage);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Gramage?> GetByNameAsync(string grm)
    {
        return await _context.Gramage.FirstOrDefaultAsync(e => e.GRM == grm);
    }

    public IQueryable<Gramage> Query() =>
        _context.Gramage
            .Select(x => new Gramage
            {
                Id = x.Id, // Add if needed
                GRM = x.GRM,
                IsActive = x.IsActive,
            });
}
