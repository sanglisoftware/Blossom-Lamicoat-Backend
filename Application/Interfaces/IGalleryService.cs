using Api.Application.DTOs;

namespace Api.Application.Interfaces;

public interface IGalleryService
{
    Task<PagedResultDto<GalleryDto>> GetAllAsync(PagedQueryDto query);
    Task<GalleryDto?> GetByIdAsync(int id);
    Task<GalleryDto> CreateAsync(GalleryDto dto);
    Task<GalleryDto?> UpdateAsync(int id, GalleryDto dto);
    Task<bool> DeleteAsync(int id);
    Task<GalleryDto?> UpdateSequenceAsync(int id, int sequenceNo);

    Task<GalleryDto?> UpdateStatusAsync(int id, short isActive);

    public Task<object> GetGalleryForWebsite();

}
