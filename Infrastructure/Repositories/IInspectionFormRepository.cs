using Api.Domain.Entities;

namespace Api.Infrastructure.Repositories;

public interface IInspectionFormRepository
{
    IQueryable<InspectionForm> Query();
    Task<IEnumerable<InspectionForm>> GetAllAsync();
    Task<InspectionForm?> GetByIdAsync(int id);
    Task<InspectionForm> AddAsync(InspectionForm inspectionForm);
}
