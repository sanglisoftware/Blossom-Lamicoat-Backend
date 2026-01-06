using Api.Domain.Entities;
namespace Api.Infrastructure.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    IQueryable<Product> Query();
    IQueryable<Product> QueryWithCategory();
    Task<Product?> GetByIdAsync(int id);
    Task<Product> AddAsync(Product product);
    Task<Product?> UpdateAsync(int id, Product product);
    Task<bool> DeleteAsync(int id);
    Task<List<Product>> ChangeStatus(int id, short isActive);

}
