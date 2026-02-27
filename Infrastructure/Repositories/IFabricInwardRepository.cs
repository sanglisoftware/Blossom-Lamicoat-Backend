using Api.Domain.Entities;

namespace Api.Infrastructure.Repositories;

public interface IFabricInwardRepository
{
    IQueryable<FabricInward> Query();
    Task<IEnumerable<FabricInward>> GetAllAsync();
    Task<FabricInward?> GetByIdAsync(int id);
    Task<FabricInward> AddAsync(FabricInward fabricinward);
    Task<FabricInward?> UpdateAsync(int id, FabricInward fabricinward);
    Task<bool> DeleteAsync(int id);
}
