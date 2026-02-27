using Api.Domain.Entities;

namespace Api.Infrastructure.Repositories;

public interface IFinalProductRepository
{
    IQueryable<FinalProduct> Query();
    Task<IEnumerable<FinalProduct>> GetAllAsync();
    Task<FinalProduct?> GetByIdAsync(int id);
    Task<FinalProduct> AddAsync(FinalProduct finalproduct);
    Task<FinalProduct?> UpdateAsync(int id, FinalProduct finalproduct);
    Task<bool> DeleteAsync(int id);
}
