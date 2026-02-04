using Api.Application.DTOs;

namespace Api.Application.Interfaces;

public interface IFGramageService
{
    Task<PagedResultDto<FGramageDto>> GetAllAsync(PagedQueryDto query);
    Task<FGramageDto?> GetByIdAsync(int id);
    Task<FGramageDto> CreateAsync(FGramageDto dto);
    Task<FGramageDto?> UpdateAsync(int id, FGramageDto dto);
    Task<bool> DeleteAsync(int id);
    Task<FGramageDto?> UpdateStatusAsync(int id, short IsActive);
}
