using Api.Application.DTOs;

namespace Api.Application.Interfaces;

public interface ICategoryService
{
    Task<PagedResultDto<CategoryDto>> GetAllAsync(PagedQueryDto query);
    Task<CategoryDto?> GetByIdAsync(int id);
    Task<CategoryDto> CreateAsync(CategoryDto dto);
    Task<CategoryDto?> UpdateAsync(int id, CategoryDto dto);
    Task<bool> DeleteAsync(int id);
    Task<CategoryDto?> UpdateSequenceAsync(int id, int sequenceNo);

    Task<CategoryDto?> UpdateStatusAsync(int id, short isActive);

    Task<List<object>> GetAllCategoriesWebsite();
}
