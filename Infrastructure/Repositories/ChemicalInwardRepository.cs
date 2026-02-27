using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class ChemicalInwardRepository
    : IChemicalInwardRepository
{
    private readonly AppDbContext _context;

    public ChemicalInwardRepository(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<ChemicalInward> Query()
    {
        return _context.ChemicalInward
            .Include(x => x.Chemical)
            .Include(x => x.Supplier);
               
    }

    public async Task<ChemicalInward?> GetByIdAsync(int id)
    {
        return await _context.ChemicalInward
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<ChemicalInward> AddAsync(
        ChemicalInward entity)
    {
        await _context.ChemicalInward.AddAsync(entity);
        return entity;
    }

    public async Task<ChemicalInward?> UpdateAsync(
        int id,
        ChemicalInward entity)
    {
        var existing = await _context.ChemicalInward
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

    public Task<IEnumerable<ChemicalInward>> GetAllAsync()
    {
        throw new NotImplementedException();
    }
}
