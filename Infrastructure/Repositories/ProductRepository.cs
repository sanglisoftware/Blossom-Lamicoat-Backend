using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class ProductRepository(AppDbContext _context) : IProductRepository
{

    public async Task<IEnumerable<Product>> GetAllAsync() => await _context.Products.ToListAsync();
    public async Task<Product?> GetByIdAsync(int id) => await _context.Products.FindAsync(id);

    public IQueryable<Product> Query() { return _context.Products.AsQueryable(); }

    public IQueryable<Product> QueryWithCategory() { return _context.Products.Include(p => p.Category); }

    public async Task<Product> AddAsync(Product product)
    {
        product.Id = 0;
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product?> UpdateAsync(int id, Product product)
    {
        var existing = await _context.Products.FindAsync(id);
        if (existing == null) return null;

        _context.Entry(existing).CurrentValues.SetValues(product);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Product>> ChangeStatus(int id, short isActive)
    {
        //get all products under category (to change status of all products under that category)
        var products = await _context.Products.Where(p => p.CategoryId == id).ToListAsync();

        foreach (var product in products)
        {
            product.IsActive = isActive;
        }
        await _context.SaveChangesAsync();
        return products;
    }
}