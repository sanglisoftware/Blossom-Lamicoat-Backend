using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class QualityRepository
    : IQualityRepository
{
    private readonly AppDbContext _context;

    public QualityRepository(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<Quality> Query()
    {
        return _context.Quality
            .Include(x => x.Colour)
            .Include(x => x.GSM);

               
    }

    public async Task<Quality?> GetByIdAsync(int id)
    {
        return await _context.Quality
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Quality> AddAsync(
        Quality entity)
    {
        await _context.Quality.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Quality?> UpdateAsync(
        int id,
        Quality entity)
    {
        var existing = await _context.Quality
            .FirstOrDefaultAsync(x => x.Id == id);

        if (existing == null)
            return null;

        _context.Entry(existing).CurrentValues.SetValues(entity);

        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.Quality
            .FirstOrDefaultAsync(x => x.Id == id);

        if (existing == null)
            return false;

        _context.Quality.Remove(existing);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<Quality>> GetAllAsync()
    {
        return await _context.Quality
            .Include(x => x.Colour)
            .Include(x => x.GSM)
            .ToListAsync();
    }
}
