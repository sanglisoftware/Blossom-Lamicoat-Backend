
using Api.Domain.Entities;

namespace Api.Infrastructure.Repositories;

public interface INewsRepository
{
    Task<IEnumerable<News>> GetAllAsync();
    IQueryable<News> Query();
    Task<News?> GetByIdAsync(int id);
    Task AddAsync(News News);
    Task<News?> UpdateAsync(int id, News updated);
    Task<bool> DeleteAsync(int id);
}
