using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace Api.Infrastructure.Repositories;

public class CategoryRepository(AppDbContext _db) : ICategoryRepository
{
    public async Task<IEnumerable<Category>> GetAllAsync() => await _db.Categories.ToListAsync();

    public IQueryable<Category> Query()
            => _db.Categories.AsNoTracking();

    public async Task<Category?> GetByIdAsync(int id) => await _db.Categories.FindAsync(id);

    public async Task AddAsync(Category Category)
    {
        _db.Categories.Add(Category);
        await _db.SaveChangesAsync();
    }

    public async Task<Category?> UpdateAsync(int id, Category updated)
    {
        var Category = await _db.Categories.FindAsync(id);
        if (Category is null) return null;

        Category.Name = updated.Name;
        Category.SequenceNo = updated.SequenceNo;
        Category.IsActive = updated.IsActive;
        Category.ImagePath = updated.ImagePath;

        await _db.SaveChangesAsync();
        return Category;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var Category = await _db.Categories.FindAsync(id);
        if (Category is null) return false;

        _db.Categories.Remove(Category);
        await _db.SaveChangesAsync();
        return true;
    }
}
