using Api.Domain.Entities;

namespace Api.Infrastructure.Repositories;

public interface IQualityRepository
{
    IQueryable<Quality> Query();
    Task<IEnumerable<Quality>> GetAllAsync();
    Task<Quality?> GetByIdAsync(int id);
    Task<Quality> AddAsync(Quality quality);
    Task<Quality?> UpdateAsync(int id, Quality quality);
    Task<bool> DeleteAsync(int id);
}


