using Api.Application.DTOs;

namespace Api.Application.Interfaces;

public interface INutritionService
{
    Task SaveNutritionDataAsync(ProductNutritionDto dto);
    Task<ProductNutritionDto?> GetNutritionDataAsync(int productId);
    Task<NutritionWebsiteResponseDto?> GetWebsiteNutritionDataAsync(int productId);
}
