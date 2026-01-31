using Api.Application.DTOs;

namespace Api.Application.Interfaces;

public interface IColourService
{
    Task<PagedResultDto<ColourDto>> GetAllAsync(PagedQueryDto query);
    Task<ColourDto?> GetByIdAsync(int id);
    Task<ColourDto> CreateAsync(ColourDto dto);
    Task<ColourDto?> UpdateAsync(int id, ColourDto dto);
    Task<bool> DeleteAsync(int id);
    Task<ColourDto?> UpdateStatusAsync(int id, short IsActive);
}
