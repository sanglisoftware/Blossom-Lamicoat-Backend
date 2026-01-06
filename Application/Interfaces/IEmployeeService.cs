using Api.Application.DTOs;

namespace Api.Application.Interfaces;

public interface IEmployeeService
{
    Task<PagedResultDto<EmployeeDto>> GetAllAsync(PagedQueryDto query);
    Task<EmployeeDto?> GetByIdAsync(int id);
    Task<EmployeeDto> CreateAsync(EmployeeDto dto);
    Task<EmployeeDto?> UpdateAsync(int id, EmployeeDto dto);
    Task<bool> DeleteAsync(int id);
    Task<EmployeeDto?> UpdateStatusAsync(int id, short isActive);
}
