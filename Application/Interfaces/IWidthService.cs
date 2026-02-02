using Api.Application.DTOs;

namespace Api.Application.Interfaces;

public interface IWidthService
{
    Task<PagedResultDto<WidthDto>> GetAllAsync(PagedQueryDto query);
    Task<WidthDto?> GetByIdAsync(int id);
    Task<WidthDto> CreateAsync(WidthDto dto);
    Task<WidthDto?> UpdateAsync(int id, WidthDto dto);
    Task<bool> DeleteAsync(int id);
    Task<WidthDto?> UpdateStatusAsync(int id, short IsActive);
}
