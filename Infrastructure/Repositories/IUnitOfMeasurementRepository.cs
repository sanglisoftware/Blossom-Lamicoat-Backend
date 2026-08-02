using Api.Domain.Entities;

namespace Api.Infrastructure.Repositories;

public interface IUnitOfMeasurementRepository
{
    IQueryable<UnitOfMeasurement> Query();
    Task<UnitOfMeasurement?> GetByIdAsync(int id);
    Task<UnitOfMeasurement> AddAsync(UnitOfMeasurement unitOfMeasurement);
    Task<UnitOfMeasurement?> UpdateAsync(int id, UnitOfMeasurement unitOfMeasurement);
    Task<bool> DeleteAsync(int id);
}
