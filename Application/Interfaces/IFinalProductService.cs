using Api.Application.DTOs;

namespace Api.Application.Interfaces;

public interface IFinalProductService
{
    Task<PagedResultDto<FinalProductDto>> GetAllAsync(PagedQueryDto query);
    Task<FinalProductDto?> GetByIdAsync(int id);
    Task<FinalProductDto> CreateAsync(FinalProductDto dto);
    Task<FinalProductDto?> UpdateAsync(int id, FinalProductDto dto);
    Task<bool> DeleteAsync(int id);
    Task<FinalProductDto?> UpdateStatusAsync(int id, short IsActive);
}
