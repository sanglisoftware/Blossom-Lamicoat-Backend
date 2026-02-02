using Api.Domain.Entities;

namespace Api.Infrastructure.Repositories;

public interface IGramageRepository
{
    IQueryable<Gramage> Query();
    Task<IEnumerable<Gramage>> GetAllAsync();
    Task<Gramage?> GetByIdAsync(int id);
    Task<Gramage> AddAsync(Gramage gramage);
    Task<Gramage?> UpdateAsync(int id, Gramage gramage);
    Task<bool> DeleteAsync(int id);
}
