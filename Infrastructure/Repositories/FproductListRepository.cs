using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class FproductListRepository
    : IFproductListRepository
{
    private readonly AppDbContext _context;

    public FproductListRepository(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<FproductList> Query()
    {
        return _context.FproductList
            .Include(x => x.Colour)
            .Include(x => x.FGramage);

               
    }

    public async Task<FproductList?> GetByIdAsync(int id)
    {
        return await _context.FproductList
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<FproductList> AddAsync(
        FproductList entity)
    {
        await _context.FproductList.AddAsync(entity);
        return entity;
    }

    public async Task<FproductList?> UpdateAsync(
        int id,
        FproductList entity)
    {
        var existing = await _context.FproductList
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

    public async Task<IEnumerable<FproductList>> GetAllAsync()
    {
        return await _context.FproductList
            .Include(x => x.Colour)
            .Include(x => x.FGramage)
            .ToListAsync();
    }
}
