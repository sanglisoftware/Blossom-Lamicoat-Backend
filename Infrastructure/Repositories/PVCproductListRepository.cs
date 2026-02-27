using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class PVCproductListRepository
    : IPVCproductListRepository
{
    private readonly AppDbContext _context;

    public PVCproductListRepository(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<PVCproductList> Query()
    {
        return _context.PVCproductList
            .Include(x => x.Colour)
            .Include(x => x.Gramage)
            .Include(x => x.Width);
               
    }

    public async Task<PVCproductList?> GetByIdAsync(int id)
    {
        return await _context.PVCproductList
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<PVCproductList> AddAsync(
        PVCproductList entity)
    {
        await _context.PVCproductList.AddAsync(entity);
        return entity;
    }

    public async Task<PVCproductList?> UpdateAsync(
        int id,
        PVCproductList entity)
    {
        var existing = await _context.PVCproductList
            .FirstOrDefaultAsync(x => x.Id == id);

        if (existing == null)
            return null;

        _context.Entry(existing).CurrentValues.SetValues(entity);

        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.FormulaChemicalTransaction
            .FirstOrDefaultAsync(x => x.Id == id);

        if (existing == null)
            return false;

        _context.FormulaChemicalTransaction.Remove(existing);

        return true;
    }

    public Task<IEnumerable<PVCproductList>> GetAllAsync()
    {
        throw new NotImplementedException();
    }
}
