using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class GalleryFilterRepository(AppDbContext _context) : IGalleryFilterRepository
{
    public async Task<GalleryFilter> CreateAsync(GalleryFilter galleryFilter)
    {
        _context.GalleryFilters.Add(galleryFilter);
        await _context.SaveChangesAsync();
        return galleryFilter;
    }
    public IQueryable<GalleryFilter> Query() => _context.GalleryFilters.AsNoTracking();
    public async Task<IEnumerable<GalleryFilter>> GetAllAsync() => await _context.GalleryFilters.ToListAsync();

    public async Task<GalleryFilter?> GetByIdAsync(int id) => await _context.GalleryFilters.FindAsync(id);

    public async Task<GalleryFilter?> UpdateAsync(int id, GalleryFilter galleryFilter)
    {
        var existing = await _context.GalleryFilters.FindAsync(id);
        if (existing == null) return null;

        existing.FilterValue = galleryFilter.FilterValue;
        await _context.SaveChangesAsync();
        return existing;
    }
    public async Task<bool> DeleteAsync(int id)
    {
        var galleryFilter = await _context.GalleryFilters.FindAsync(id);
        if (galleryFilter == null) return false;

        _context.GalleryFilters.Remove(galleryFilter);
        await _context.SaveChangesAsync();
        return true;
    }
}