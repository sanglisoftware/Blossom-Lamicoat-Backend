using Api.Application.DTOs;

namespace Api.Application.Interfaces;

public interface IPVCInwardService
{
    Task<PagedResultDto<PVCInwardDto>> GetAllAsync(PagedQueryDto query);
    Task<PVCInwardDto?> GetByIdAsync(int id);
    Task<PVCInwardDto> CreateAsync(PVCInwardDto dto);
    Task<PVCInwardDto?> UpdateAsync(int id, PVCInwardDto dto);
    Task<bool> DeleteAsync(int id);
    Task<PVCInwardDto?> UpdateStatusAsync(int id, short IsActive);
}


