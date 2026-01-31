using Api.Application.DTOs;

namespace Api.Application.Interfaces;

public interface ISupplierService
{
    Task<PagedResultDto<SupplierDto>> GetAllAsync(PagedQueryDto query);
    Task<SupplierDto?> GetByIdAsync(int id);
    Task<SupplierDto> CreateAsync(SupplierDto dto);
    Task<SupplierDto?> UpdateAsync(int id, SupplierDto dto);
    Task<bool> DeleteAsync(int id);
    Task<SupplierDto?> UpdateStatusAsync(int id, short IsActive);
}
