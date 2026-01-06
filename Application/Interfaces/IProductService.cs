using Api.Application.DTOs;
using Api.Domain.Entities;

namespace Api.Application.Interfaces;

public interface IProductService
{
    Task<PagedResultDto<ProductDto>> GetAllAsync(PagedQueryDto query);
    Task<ProductDto?> GetByIdAsync(int id);
    Task<ProductDto> CreateAsync(ProductDto dto);
    Task<ProductDto?> UpdateAsync(int id, ProductDto dto);
    Task<bool> DeleteAsync(int id);
    Task<List<Product>> ChangeProductStatus(int id, short isActive);
    Task<ProductDto?> UpdateSequenceAsync(int id, int sequenceNo);
    Task<ProductDto?> UpdateStatusAsync(int id, short isActive);

    Task<List<object>> GetAllFeaturedProductsWebsite();
    Task<dynamic> GetAllProductsWebsite(int id, int category, string? search, int pageSize, int page);
    Task<List<object>> GetRelatedProductsWebsite(int productId);
}
