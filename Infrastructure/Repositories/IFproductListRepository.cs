using Api.Domain.Entities;

namespace Api.Infrastructure.Repositories;

public interface IFproductListRepository
{
    IQueryable<FproductList> Query();
    Task<IEnumerable<FproductList>> GetAllAsync();
    Task<FproductList?> GetByIdAsync(int id);
    Task<FproductList> AddAsync(FproductList fproductlist);
    Task<FproductList?> UpdateAsync(int id, FproductList fproductlist);
    Task<bool> DeleteAsync(int id);
}
