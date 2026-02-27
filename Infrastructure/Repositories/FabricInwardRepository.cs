using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class FabricInwardRepository
    : IFabricInwardRepository
{
    private readonly AppDbContext _context;

    public FabricInwardRepository(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<FabricInward> Query()
    {
        return _context.FabricInward
            .Include(x => x.Supplier)
            .Include(x => x.Fabric)
                .AsNoTracking();
    }

    public async Task<FabricInward?> GetByIdAsync(int id)
    {
        return await _context.FabricInward
            .Include(x => x.Supplier)
            .Include(x => x.Fabric)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<FabricInward> AddAsync(
        FabricInward entity)
    {
        await _context.FabricInward.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<FabricInward?> UpdateAsync(
        int id,
        FabricInward entity)
    {
        var existing = await _context.FabricInward
            .FirstOrDefaultAsync(x => x.Id == id);

        if (existing == null)
            return null;

        _context.Entry(existing).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.FabricInward
            .FirstOrDefaultAsync(x => x.Id == id);

        if (existing == null)
            return false;

        _context.FabricInward.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }

    public Task<IEnumerable<FabricInward>> GetAllAsync()
    {
        throw new NotImplementedException();
    }
}
