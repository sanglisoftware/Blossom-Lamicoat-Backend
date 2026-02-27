using Api.Domain.Entities;

namespace Api.Infrastructure.Repositories;

public interface IPVCInwardRepository
{
    IQueryable<PVCInward> Query();
    Task<IEnumerable<PVCInward>> GetAllAsync();
    Task<PVCInward?> GetByIdAsync(int id);
    Task<PVCInward> AddAsync(PVCInward pvcinward);
    Task<PVCInward?> UpdateAsync(int id, PVCInward pvcinward);
    Task<bool> DeleteAsync(int id);
}
