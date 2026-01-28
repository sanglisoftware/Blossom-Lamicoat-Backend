using Api.Domain.Entities;

namespace Api.Infrastructure.Repositories;

public interface IChemicalRepository
{
    IQueryable<Chemical> Query();
    Task<IEnumerable<Chemical>> GetAllAsync();
    Task<Chemical?> GetByIdAsync(int id);
    Task<Chemical> AddAsync(Chemical chemical);
    Task<Chemical?> UpdateAsync(int id, Chemical chemical);
    Task<bool> DeleteAsync(int id);
}
