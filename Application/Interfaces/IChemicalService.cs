using Api.Application.DTOs;

namespace Api.Application.Interfaces;

public interface IChemicalService
{
    Task<PagedResultDto<ChemicalDto>> GetAllAsync(PagedQueryDto query);
    Task<ChemicalDto?> GetByIdAsync(int id);
    Task<ChemicalDto> CreateAsync(ChemicalDto dto);
    Task<ChemicalDto?> UpdateAsync(int id, ChemicalDto dto);
    Task<bool> DeleteAsync(int id);
    Task<ChemicalDto?> UpdateStatusAsync(int id, short IsActive);
}
