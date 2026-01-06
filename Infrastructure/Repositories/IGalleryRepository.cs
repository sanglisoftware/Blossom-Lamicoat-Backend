using Api.Domain.Entities;

namespace Api.Infrastructure.Repositories;

public interface IGalleryRepository
{
    Task<IEnumerable<Gallery>> GetAllAsync();
    IQueryable<Gallery> Query();
    Task<Gallery?> GetByIdAsync(int id);
    Task AddAsync(Gallery Gallery);
    Task<Gallery?> UpdateAsync(int id, Gallery updated);
    Task<bool> DeleteAsync(int id);

}
