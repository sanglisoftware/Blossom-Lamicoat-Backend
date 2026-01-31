using Api.Application.DTOs;

namespace Api.Application.Interfaces;

public interface IGradeService
{
    Task<PagedResultDto<GradeDto>> GetAllAsync(PagedQueryDto query);
    Task<GradeDto?> GetByIdAsync(int id);
    Task<GradeDto> CreateAsync(GradeDto dto);
    Task<GradeDto?> UpdateAsync(int id, GradeDto dto);
    Task<bool> DeleteAsync(int id);
    Task<GradeDto?> UpdateStatusAsync(int id, short IsActive);
}
