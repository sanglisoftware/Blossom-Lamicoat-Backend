using Api.Application.DTOs;

namespace Api.Application.Interfaces;

public interface ISalaryService
{
    Task<PagedResultDto<SalaryDto>> GetAllAsync(PagedQueryDto query);
    Task<SalaryDto?> GetByIdAsync(int id);
    Task<SalaryDto> CreateAsync(SalaryDto dto);
}
