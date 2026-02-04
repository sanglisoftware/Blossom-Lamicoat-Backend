using Api.Application.DTOs;

namespace Api.Application.Interfaces;

public interface IGSMService
{
    Task<PagedResultDto<GSMDto>> GetAllAsync(PagedQueryDto query);
    Task<GSMDto?> GetByIdAsync(int id);
    Task<GSMDto> CreateAsync(GSMDto dto);
    Task<GSMDto?> UpdateAsync(int id, GSMDto dto);
    Task<bool> DeleteAsync(int id);
    Task<GSMDto?> UpdateStatusAsync(int id, short IsActive);
}
