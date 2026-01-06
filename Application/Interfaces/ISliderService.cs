using Api.Application.DTOs;

namespace Api.Application.Interfaces;

public interface ISliderService
{
    Task<PagedResultDto<SliderDto>> GetAllAsync(PagedQueryDto query);
    Task<SliderDto?> GetByIdAsync(int id);
    Task<SliderDto> CreateAsync(SliderDto dto);
    Task<SliderDto?> UpdateAsync(int id, SliderDto dto);
    Task<bool> DeleteAsync(int id);
    Task<SliderDto?> UpdateSequenceAsync(int id, int sequenceNo);

    Task<SliderDto?> UpdateStatusAsync(int id, short isActive);

    Task<List<string>> GetSliderImagesForWebsite();
}
