using System.Text.Json;
using Api.Application.DTOs;
using Api.Application.Interfaces;
using Api.Domain.Entities;
using Api.Infrastructure.Repositories;

namespace Api.Application.Services
{
    public class NutritionService(IProductNutritionRepository nutritionRepository) : INutritionService
    {
        private readonly IProductNutritionRepository _nutritionRepository = nutritionRepository;

        public async Task SaveNutritionDataAsync(ProductNutritionDto dto)
        {
            var nutritionData = JsonSerializer.Serialize(dto.Nutrition);
            var existing = await _nutritionRepository.GetByProductIdAsync(dto.ProductId);

            if (existing == null)
            {
                var newNutrition = new ProductNutrition
                {
                    ProductId = dto.ProductId,
                    NutritionalValue = nutritionData,
                    CreatedDate = DateTime.UtcNow,
                };
                await _nutritionRepository.AddAsync(newNutrition);
            }
            else
            {
                existing.NutritionalValue = nutritionData;
                await _nutritionRepository.UpdateAsync(existing);
            }
        }

        public async Task<ProductNutritionDto?> GetNutritionDataAsync(int productId)
        {
            var nutrition = await _nutritionRepository.GetByProductIdAsync(productId);
            if (nutrition == null)
                return null;

            return new ProductNutritionDto
            {
                ProductId = productId,
                Nutrition = JsonSerializer.Deserialize<NutritionDto>(nutrition.NutritionalValue)!,
            };
        }

        public async Task<NutritionWebsiteResponseDto?> GetWebsiteNutritionDataAsync(int productId)
        {
            var nutrition = await _nutritionRepository.GetByProductIdAsync(productId);
            if (nutrition == null)
                return null;

            // Deserialize to the original DTO
            var nutritionDto = JsonSerializer.Deserialize<NutritionDto>(nutrition.NutritionalValue);
            if (nutritionDto == null)
                return null;

            // Map to the new response DTO
            return new NutritionWebsiteResponseDto
            {
                Title = nutritionDto.Title,
                ServingSize = nutritionDto.ServingSize,
                Rows = [.. nutritionDto
                    .Rows.Select(row => new NutritionWebsiteResponseRow
                    {
                        Name = row.Name,
                        Value = $"{row.Value} {row.Unit}".Trim(), // Combine value + unit
                        DailyValue = row.DailyValue,
                    })],
            };
        }
    }
}
