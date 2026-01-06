using Api.Application.DTOs;
namespace Api.Application.Interfaces;

public interface IShopService
{
    Task<ShopDto> CreateShopAsync(ShopDto request);
    Task<IEnumerable<ShopDto>> GetAllAsync();

    Task<IEnumerable<ShopResponseDto>> GetAllShopsForWebSite();

    Task<IEnumerable<ShopResponseDto>> GetSearchedShopsForWeb(string? search);

    Task<PagedResultDto<ShopDto>> GetAllAsync(PagedQueryDto query);
    Task<ShopDto?> GetByIdAsync(int id);
    Task<ShopDto?> UpdateAsync(int id, ShopDto dto);
    Task<bool> DeleteAsync(int id);

    Task<ShopDto?> UpdateStatusAsync(int id, short isActive);
}
