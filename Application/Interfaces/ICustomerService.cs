using Api.Application.DTOs;

namespace Api.Application.Interfaces;

public interface ICustomerService
{
    Task<PagedResultDto<CustomerDto>> GetAllAsync(PagedQueryDto query);
    Task<CustomerDto?> GetByIdAsync(int id);
    Task<CustomerDto> CreateAsync(CustomerDto dto);
    Task<CustomerDto?> UpdateAsync(int id, CustomerDto dto);
    Task<bool> DeleteAsync(int id);
    Task<CustomerDto?> UpdateStatusAsync(int id, short IsActive);
}
