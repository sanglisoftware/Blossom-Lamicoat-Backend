using Api.Application.DTOs;

namespace Api.Application.Interfaces;

public interface IMixtureFormService
{
    Task<PagedResultDto<MixtureFormDto>> GetAllAsync(PagedQueryDto query);
    Task<MixtureFormDto?> GetByIdAsync(int id);
    Task<MixtureFormDto> CreateAsync(MixtureFormDto dto);
}
