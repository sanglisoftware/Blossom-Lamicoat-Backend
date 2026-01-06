using Api.Domain.Entities;
namespace Api.Infrastructure.Repositories;

public interface IGalleryFilterRepository
{
    Task<IEnumerable<GalleryFilter>> GetAllAsync();
    IQueryable<GalleryFilter> Query();
    Task<GalleryFilter?> GetByIdAsync(int id);
    Task<GalleryFilter> CreateAsync(GalleryFilter galleryFilter);
    Task<GalleryFilter?> UpdateAsync(int id, GalleryFilter galleryFilter);
    Task<bool> DeleteAsync(int id);
}