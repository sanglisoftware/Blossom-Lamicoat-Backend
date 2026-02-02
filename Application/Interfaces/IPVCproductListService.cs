using Api.Application.DTOs;

namespace Api.Application.Interfaces;

public interface IPVCproductListService
{
    Task<PagedResultDto<PVCproductListDto>> GetAllAsync(PagedQueryDto query);
    Task<PVCproductListDto?> GetByIdAsync(int id);
    Task<PVCproductListDto> CreateAsync(PVCproductListDto dto);
    Task<PVCproductListDto?> UpdateAsync(int id, PVCproductListDto dto);
    Task<bool> DeleteAsync(int id);
    Task<PVCproductListDto?> UpdateStatusAsync(int id, short IsActive);
}
