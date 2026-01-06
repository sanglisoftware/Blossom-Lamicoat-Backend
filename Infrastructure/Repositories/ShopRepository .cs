using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace Api.Infrastructure.Repositories;

public class ShopRepository(AppDbContext db) : IShopRepository
{
    public async Task AddAsync(Shop shop)
    {
        await db.Shop.AddAsync(shop);
        await db.SaveChangesAsync();
    }



    public async Task<List<Shop>> GetAllAsync()
    {
        return await db.Shop.AsNoTracking().ToListAsync();
    }
    public IQueryable<Shop> Query() => db.Shop.AsNoTracking();
    public async Task<Shop?> GetByIdAsync(int id)
    {
        return await db.Shop.FindAsync(id);
    }
    public async Task UpdateAsync(Shop shop)
    {
        db.Shop.Update(shop);
        await db.SaveChangesAsync();
    }
    public async Task DeleteAsync(Shop shop)
    {
        db.Shop.Remove(shop);
        await db.SaveChangesAsync();
    }

    public async Task<IEnumerable<Shop>> GetAllShopsForWeb()
    {
        return await db.Shop.ToListAsync();
    }
}
