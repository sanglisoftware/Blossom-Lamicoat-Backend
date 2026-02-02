using Api.Application.DTOs;

namespace Api.Application.Interfaces;

public interface IGramageService
{
    Task<PagedResultDto<GramageDto>> GetAllAsync(PagedQueryDto query);
    Task<GramageDto?> GetByIdAsync(int id);
    Task<GramageDto> CreateAsync(GramageDto dto);
    Task<GramageDto?> UpdateAsync(int id, GramageDto dto);
    Task<bool> DeleteAsync(int id);
    Task<GramageDto?> UpdateStatusAsync(int id, short IsActive);
}
