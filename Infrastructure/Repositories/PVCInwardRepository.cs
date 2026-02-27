using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class PVCInwardRepository
    : IPVCInwardRepository
{
    private readonly AppDbContext _context;

    public PVCInwardRepository(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<PVCInward> Query()
    {
        return _context.PVCInward
            .Include(x => x.PVC)
            .Include(x => x.Supplier);
               
    }

    public async Task<PVCInward?> GetByIdAsync(int id)
    {
        return await _context.PVCInward
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<PVCInward> AddAsync(
        PVCInward entity)
    {
        await _context.PVCInward.AddAsync(entity);
        return entity;
    }

    public async Task<PVCInward?> UpdateAsync(
        int id,
        PVCInward entity)
    {
        var existing = await _context.PVCInward
            .FirstOrDefaultAsync(x => x.Id == id);

        if (existing == null)
            return null;

        _context.Entry(existing).CurrentValues.SetValues(entity);

        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.PVCInward
            .FirstOrDefaultAsync(x => x.Id == id);

        if (existing == null)
            return false;

        _context.PVCInward.Remove(existing);

        return true;
    }

    public Task<IEnumerable<PVCInward>> GetAllAsync()
    {
        throw new NotImplementedException();
    }
}
