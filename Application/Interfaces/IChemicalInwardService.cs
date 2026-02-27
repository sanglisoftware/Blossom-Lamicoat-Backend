using Api.Application.DTOs;

namespace Api.Application.Interfaces;

public interface IChemicalInwardService
{
    Task<PagedResultDto<ChemicalInwardDto>> GetAllAsync(PagedQueryDto query);
    Task<ChemicalInwardDto?> GetByIdAsync(int id);
    Task<ChemicalInwardDto> CreateAsync(ChemicalInwardDto dto);
    Task<ChemicalInwardDto?> UpdateAsync(int id, ChemicalInwardDto dto);
    Task<bool> DeleteAsync(int id);
    Task<ChemicalInwardDto?> UpdateStatusAsync(int id, short IsActive);
}


