using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class QualityRepository(AppDbContext _context) : IQualityRepository
{
    public async Task<IEnumerable<Quality>> GetAllAsync() => await _context.Quality.ToListAsync();

    public async Task<Quality?> GetByIdAsync(int id) => await _context.Quality.FindAsync(id);

    public async Task<Quality> AddAsync(Quality quality)
    {
        await _context.Quality.AddAsync(quality);
        return quality;
    }

    public async Task<Quality?> UpdateAsync(int id, Quality quality)
    {
        var existing = await _context.Quality.FindAsync(id);
        if (existing == null) return null;

        _context.Entry(existing).CurrentValues.SetValues(quality);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var quality = await _context.Quality.FindAsync(id);
        if (quality == null)
            return false;

        _context.Quality.Remove(quality);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Quality?> GetByNameAsync(string name)
    {
        return await _context.Quality.FirstOrDefaultAsync(e => e.Name == name);
    }

    public IQueryable<Quality> Query() =>
        _context.Quality
            .Select(x => new Quality
            {
                Id = x.Id, // Add if needed
                Name = x.Name,
                Comments = x.Comments,
                GSM_GLM = x.GSM_GLM,
                Colour = x.Colour,
                IsActive = x.IsActive,
            });
}
