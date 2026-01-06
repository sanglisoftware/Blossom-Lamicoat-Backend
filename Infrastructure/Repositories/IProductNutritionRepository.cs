
using Api.Domain.Entities;

namespace Api.Infrastructure.Repositories
{
    public interface IProductNutritionRepository
    {
        Task AddAsync(ProductNutrition productNutrition);
        Task<ProductNutrition?> GetByProductIdAsync(int productId);
        Task UpdateAsync(ProductNutrition productNutrition);
    }
}