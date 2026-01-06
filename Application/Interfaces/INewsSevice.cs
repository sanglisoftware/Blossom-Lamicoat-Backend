using Api.Application.DTOs;

namespace Api.Application.Interfaces
{
    public interface INewsService
    {
        Task<NewsCreateDto> AddNewsAsync(NewsCreateDto dto);
        Task<PagedResultDto<NewsCreateDto>> GetAllAsync(PagedQueryDto query);
        Task<NewsCreateDto?> GetByIdAsync(int id);
        Task<NewsCreateDto?> UpdateAsync(int id, NewsCreateDto dto);
        Task<NewsCreateDto?> UpdateStatusAsync(int id, short isActive);
        Task<bool> DeleteAsync(int id);
    }
}
