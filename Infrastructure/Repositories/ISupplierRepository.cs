using Api.Domain.Entities;

namespace Api.Infrastructure.Repositories;

public interface ISupplierRepository
{
    IQueryable<Supplier> Query();
    Task<IEnumerable<Supplier>> GetAllAsync();
    Task<Supplier?> GetByIdAsync(int id);
    Task<Supplier> AddAsync(Supplier supplier);
    Task<Supplier?> UpdateAsync(int id, Supplier supplier);
    Task<bool> DeleteAsync(int id);
}
