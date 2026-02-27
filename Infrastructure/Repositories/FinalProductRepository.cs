using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class FinalProductRepository(AppDbContext _context) : IFinalProductRepository
{
    public async Task<IEnumerable<FinalProduct>> GetAllAsync() => await _context.FinalProduct.ToListAsync();

    public async Task<FinalProduct?> GetByIdAsync(int id) => await _context.FinalProduct.FindAsync(id);

    public async Task<FinalProduct> AddAsync(FinalProduct finalproduct)
    {
        await _context.FinalProduct.AddAsync(finalproduct);
        return finalproduct;
    }

    public async Task<FinalProduct?> UpdateAsync(int id, FinalProduct finalproduct)
    {
        var existing = await _context.FinalProduct.FindAsync(id);
        if (existing == null) return null;

        _context.Entry(existing).CurrentValues.SetValues(finalproduct);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var finalproduct = await _context.FinalProduct.FindAsync(id);
        if (finalproduct == null)
            return false;

        _context.FinalProduct.Remove(finalproduct);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<FinalProduct?> GetByNameAsync(string finalproduct)
    {
        return await _context.FinalProduct.FirstOrDefaultAsync(e => e.Final_Product == finalproduct);
    }

    public IQueryable<FinalProduct> Query() =>
        _context.FinalProduct
            .Select(x => new FinalProduct
            {
                Id = x.Id, // Add if needed
                Final_Product = x.Final_Product,
                IsActive = x.IsActive,
            });
}
