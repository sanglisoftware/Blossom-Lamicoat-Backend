using Api.Application.DTOs;

namespace Api.Application.Interfaces;

public interface IUnitOfMeasurementService
{
    Task<PagedResultDto<UnitOfMeasurementDto>> GetAllAsync(PagedQueryDto query);
    Task<UnitOfMeasurementDto?> GetByIdAsync(int id);
    Task<UnitOfMeasurementDto> CreateAsync(UnitOfMeasurementDto dto);
    Task<UnitOfMeasurementDto?> UpdateAsync(int id, UnitOfMeasurementDto dto);
    Task<bool> DeleteAsync(int id);
    Task<UnitOfMeasurementDto?> UpdateStatusAsync(int id, short isActive);
}
