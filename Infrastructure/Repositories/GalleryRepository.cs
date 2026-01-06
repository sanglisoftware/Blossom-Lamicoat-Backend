using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace Api.Infrastructure.Repositories;

public class GalleryRepository(AppDbContext _db) : IGalleryRepository
{
    public IQueryable<Gallery> Query() => _db.Galleries.AsNoTracking();

    public async Task<IEnumerable<Gallery>> GetAllAsync() => await _db.Galleries.ToListAsync();

    public async Task<Gallery?> GetByIdAsync(int id) => await _db.Galleries.FindAsync(id);
    public async Task AddAsync(Gallery Gallery)
    {
        _db.Galleries.Add(Gallery);
        await _db.SaveChangesAsync();
    }

    public async Task<Gallery?> UpdateAsync(int id, Gallery updated)
    {
        var gallery = await _db.Galleries.FindAsync(id);
        if (gallery is null) return null;

        gallery.Title = updated.Title;
        gallery.FilterId = updated.FilterId;
        gallery.SequenceNo = updated.SequenceNo;
        gallery.IsActive = updated.IsActive;
        gallery.ImagePath = updated.ImagePath;

        await _db.SaveChangesAsync();
        return gallery;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var gallery = await _db.Galleries.FindAsync(id);
        if (gallery is null) return false;

        _db.Galleries.Remove(gallery);
        await _db.SaveChangesAsync();
        return true;
    }


}