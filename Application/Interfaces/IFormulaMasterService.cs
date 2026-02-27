using Api.Application.DTOs;

namespace Api.Application.Interfaces;

public interface IFormulaMasterService
{
    Task<PagedResultDto<FormulaMasterDto>> GetAllAsync(PagedQueryDto query);
    Task<FormulaMasterDto?> GetByIdAsync(int id);
    Task<FormulaMasterDto> CreateAsync(FormulaMasterDto dto);
    Task<FormulaMasterDto?> UpdateAsync(int id, FormulaMasterDto dto);
    Task<bool> DeleteAsync(int id);
    Task<FormulaMasterDto?> UpdateStatusAsync(int id, short IsActive);
}


