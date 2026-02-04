using Api.Application.DTOs;

namespace Api.Application.Interfaces;

public interface IFproductListService
{
    Task<PagedResultDto<FproductListDto>> GetAllAsync(PagedQueryDto query);
    Task<FproductListDto?> GetByIdAsync(int id);
    Task<FproductListDto> CreateAsync(FproductListDto dto);
    Task<FproductListDto?> UpdateAsync(int id, FproductListDto dto);
    Task<bool> DeleteAsync(int id);
    Task<FproductListDto?> UpdateStatusAsync(int id, short IsActive);
}
