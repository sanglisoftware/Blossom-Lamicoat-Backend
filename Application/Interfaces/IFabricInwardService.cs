using Api.Application.DTOs;

namespace Api.Application.Interfaces;

public interface IFabricInwardService
{
    Task<PagedResultDto<FabricInwardDto>> GetAllAsync(PagedQueryDto query);
    Task<FabricInwardDto?> GetByIdAsync(int id);
    Task<FabricInwardDto> CreateAsync(FabricInwardDto dto);
    Task<FabricInwardDto?> UpdateAsync(int id, FabricInwardDto dto);
    Task<bool> DeleteAsync(int id);
    Task<FabricInwardDto?> UpdateStatusAsync(int id, short IsActive);
}


