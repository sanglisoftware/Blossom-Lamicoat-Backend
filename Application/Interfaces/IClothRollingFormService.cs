using Api.Application.DTOs;

namespace Api.Application.Interfaces;

public interface IClothRollingFormService
{
    Task<PagedResultDto<ClothRollingFormDto>> GetAllAsync(PagedQueryDto query);
    Task<ClothRollingFormDto?> GetByIdAsync(int id);
    Task<ClothRollingFormDto> CreateAsync(ClothRollingFormDto dto);
}
