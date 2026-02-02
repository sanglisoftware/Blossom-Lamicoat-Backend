using Api.Domain.Entities;

namespace Api.Infrastructure.Repositories;

public interface IPVCproductListRepository
{
    IQueryable<PVCproductList> Query();
    Task<IEnumerable<PVCproductList>> GetAllAsync();
    Task<PVCproductList?> GetByIdAsync(int id);
    Task<PVCproductList> AddAsync(PVCproductList pvcproductlist);
    Task<PVCproductList?> UpdateAsync(int id, PVCproductList pvcproductlist);
    Task<bool> DeleteAsync(int id);
}
