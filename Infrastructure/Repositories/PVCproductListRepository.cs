using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class PVCproductListRepository(AppDbContext _context) : IPVCproductListRepository
{
    public async Task<IEnumerable<PVCproductList>> GetAllAsync() => await _context.PVCproductList.ToListAsync();

    public async Task<PVCproductList?> GetByIdAsync(int id) => await _context.PVCproductList.FindAsync(id);

    public async Task<PVCproductList> AddAsync(PVCproductList pvcproductlist)
    {
        await _context.PVCproductList.AddAsync(pvcproductlist);
        return pvcproductlist;
    }

    public async Task<PVCproductList?> UpdateAsync(int id, PVCproductList pvcproductlist)
    {
        var existing = await _context.PVCproductList.FindAsync(id);
        if (existing == null) return null;

        _context.Entry(existing).CurrentValues.SetValues(pvcproductlist);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var pvcproductlist = await _context.PVCproductList.FindAsync(id);
        if (pvcproductlist == null)
            return false;

        _context.PVCproductList.Remove(pvcproductlist);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<PVCproductList?> GetByNameAsync(string name)
    {
        return await _context.PVCproductList.FirstOrDefaultAsync(e => e.Name == name);
    }

    public IQueryable<PVCproductList> Query() =>
        _context.PVCproductList
            .Select(x => new PVCproductList
            {
                Id = x.Id, // Add if needed
                Name = x.Name,
                Gramage = x.Gramage,
                Width = x.Width,
                Colour = x.Colour,
                Comments = x.Comments,
                IsActive = x.IsActive,
            });
}
