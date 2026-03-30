using Api.Application.DTOs;

namespace Api.Application.Interfaces;

public interface IInspectionFormService
{
    Task<PagedResultDto<InspectionFormDto>> GetAllAsync(PagedQueryDto query);
    Task<InspectionFormDto?> GetByIdAsync(int id);
    Task<InspectionFormDto> CreateAsync(InspectionFormDto dto);
}
