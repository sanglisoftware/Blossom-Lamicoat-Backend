using Api.Application.DTOs;

namespace Api.Application.Interfaces;

public interface ILaminationFormService
{
    Task<PagedResultDto<LaminationFormDto>> GetAllAsync(PagedQueryDto query);
    Task<LaminationFormDto?> GetByIdAsync(int id);
    Task<LaminationFormDto> CreateAsync(LaminationFormDto dto);
}
