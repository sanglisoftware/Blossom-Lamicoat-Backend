using Api.Domain.Entities;

namespace Api.Infrastructure.Repositories;

public interface IWidthRepository
{
    IQueryable<Width> Query();
    Task<IEnumerable<Width>> GetAllAsync();
    Task<Width?> GetByIdAsync(int id);
    Task<Width> AddAsync(Width width);
    Task<Width?> UpdateAsync(int id, Width width);
    Task<bool> DeleteAsync(int id);
}
