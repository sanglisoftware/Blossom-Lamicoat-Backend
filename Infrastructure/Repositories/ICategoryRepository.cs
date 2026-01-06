using Api.Domain.Entities;

namespace Api.Infrastructure.Repositories;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync();
    IQueryable<Category> Query();
    Task<Category?> GetByIdAsync(int id);
    Task AddAsync(Category Category);
    Task<Category?> UpdateAsync(int id, Category updated);
    Task<bool> DeleteAsync(int id);
}
