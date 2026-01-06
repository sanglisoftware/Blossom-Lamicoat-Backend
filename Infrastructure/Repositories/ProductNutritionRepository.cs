using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class ProductNutritionRepository(AppDbContext _context) : IProductNutritionRepository
{
    public async Task AddAsync(ProductNutrition productNutrition)
    {
        await _context.ProductNutritions.AddAsync(productNutrition);
        await _context.SaveChangesAsync();
    }

    public async Task<ProductNutrition?> GetByProductIdAsync(int productId)
    {
        return await _context.ProductNutritions.FirstOrDefaultAsync(pn =>
            pn.ProductId == productId
        );
    }

    public async Task UpdateAsync(ProductNutrition productNutrition)
    {
        _context.ProductNutritions.Update(productNutrition);
        await _context.SaveChangesAsync();
    }
}
