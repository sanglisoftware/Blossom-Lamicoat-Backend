using Api.Application.DTOs;

namespace Api.Application.Interfaces;

public interface IQualityService
{
    Task<PagedResultDto<QualityDto>> GetAllAsync(PagedQueryDto query);
    Task<QualityDto?> GetByIdAsync(int id);
    Task<QualityDto> CreateAsync(QualityDto dto);
    Task<QualityDto?> UpdateAsync(int id, QualityDto dto);
    Task<bool> DeleteAsync(int id);
    Task<QualityDto?> UpdateStatusAsync(int id, short IsActive);
}
