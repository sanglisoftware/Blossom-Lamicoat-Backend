using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class FGramageRepository(AppDbContext _context) : IFGramageRepository
{
    public async Task<IEnumerable<FGramage>> GetAllAsync() => await _context.FGramage.ToListAsync();

    public async Task<FGramage?> GetByIdAsync(int id) => await _context.FGramage.FindAsync(id);

    public async Task<FGramage> AddAsync(FGramage fgramage)
    {
        await _context.FGramage.AddAsync(fgramage);
        return fgramage;
    }

    public async Task<FGramage?> UpdateAsync(int id, FGramage fgramage)
    {
        var existing = await _context.FGramage.FindAsync(id);
        if (existing == null) return null;

        _context.Entry(existing).CurrentValues.SetValues(fgramage);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var fgramage = await _context.FGramage.FindAsync(id);
        if (fgramage == null)
            return false;

        _context.FGramage.Remove(fgramage);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<FGramage?> GetByNameAsync(string grm)
    {
        return await _context.FGramage.FirstOrDefaultAsync(e => e.GRM == grm);
    }

    public IQueryable<FGramage> Query() =>
        _context.FGramage
            .Select(x => new FGramage
            {
                Id = x.Id, // Add if needed
                GRM = x.GRM,
                IsActive = x.IsActive,
            });
}
