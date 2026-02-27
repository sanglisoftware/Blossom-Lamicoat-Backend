using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class FormulaChemicalTransactionRepository
    : IFormulaChemicalTransactionRepository
{
    private readonly AppDbContext _context;

    public FormulaChemicalTransactionRepository(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<FormulaChemicalTransaction> Query()
    {
        return _context.FormulaChemicalTransaction
            .Include(x => x.Chemical)
            .Include(x => x.FormulaMaster)
                .ThenInclude(f => f.FinalProduct);
    }

    public async Task<FormulaChemicalTransaction?> GetByIdAsync(int id)
    {
        return await _context.FormulaChemicalTransaction
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<FormulaChemicalTransaction> AddAsync(
        FormulaChemicalTransaction entity)
    {
        await _context.FormulaChemicalTransaction.AddAsync(entity);
        return entity;
    }

    public async Task<FormulaChemicalTransaction?> UpdateAsync(
        int id,
        FormulaChemicalTransaction entity)
    {
        var existing = await _context.FormulaChemicalTransaction
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
}
