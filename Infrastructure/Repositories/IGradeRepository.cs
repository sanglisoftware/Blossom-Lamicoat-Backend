using Api.Domain.Entities;

namespace Api.Infrastructure.Repositories;

public interface IGradeRepository
{
    IQueryable<Grade> Query();
    Task<IEnumerable<Grade>> GetAllAsync();
    Task<Grade?> GetByIdAsync(int id);
    Task<Grade> AddAsync(Grade grade);
    Task<Grade?> UpdateAsync(int id, Grade grade);
    Task<bool> DeleteAsync(int id);
}
