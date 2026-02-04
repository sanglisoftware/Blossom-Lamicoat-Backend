using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class FproductListRepository(AppDbContext _context) : IFproductListRepository
{
    public async Task<IEnumerable<FproductList>> GetAllAsync() => await _context.FproductList.ToListAsync();

    public async Task<FproductList?> GetByIdAsync(int id) => await _context.FproductList.FindAsync(id);

    public async Task<FproductList> AddAsync(FproductList fproductlist)
    {
        await _context.FproductList.AddAsync(fproductlist);
        return fproductlist;
    }

    public async Task<FproductList?> UpdateAsync(int id, FproductList fproductlist)
    {
        var existing = await _context.FproductList.FindAsync(id);
        if (existing == null) return null;

        _context.Entry(existing).CurrentValues.SetValues(fproductlist);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var fproductlist = await _context.FproductList.FindAsync(id);
        if (fproductlist == null)
            return false;

        _context.FproductList.Remove(fproductlist);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<FproductList?> GetByNameAsync(string name)
    {
        return await _context.FproductList.FirstOrDefaultAsync(e => e.Name == name);
    }

    public IQueryable<FproductList> Query() =>
        _context.FproductList
            .Select(x => new FproductList
            {
                Id = x.Id, // Add if needed
                Name = x.Name,
                GRM = x.GRM,
                Colour = x.Colour,
                Comments = x.Comments,
                IsActive = x.IsActive,
            });
}
