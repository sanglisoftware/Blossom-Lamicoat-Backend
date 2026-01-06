using Api.Application.DTOs;
namespace Api.Application.Interfaces;

public interface IGalleryFilterService
{
    Task<PagedResultDto<GalleryFilterDto>> GetAllAsync(PagedQueryDto query);
    Task<IEnumerable<GalleryFilterDto>> GetAllAsync();
    Task<GalleryFilterDto?> GetByIdAsync(int id);
    Task<GalleryFilterDto> CreateAsync(GalleryFilterDto dto);
    Task<GalleryFilterDto?> UpdateAsync(int id, GalleryFilterDto dto);
    Task<bool> DeleteAsync(int id);
}
