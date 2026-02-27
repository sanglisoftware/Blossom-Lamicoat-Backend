using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class FormulaMasterRepository(AppDbContext _context) : IFormulaMasterRepository
{
    public async Task<IEnumerable<FormulaMaster>> GetAllAsync() => await _context.FormulaMaster.ToListAsync();

    public async Task<FormulaMaster?> GetByIdAsync(int id) => await _context.FormulaMaster.FindAsync(id);

    public async Task<FormulaMaster> AddAsync(FormulaMaster formulamaster)
    {
        await _context.FormulaMaster.AddAsync(formulamaster);
        return formulamaster;
    }

    public async Task<FormulaMaster?> UpdateAsync(int id, FormulaMaster formulamaster)
    {
        var existing = await _context.FormulaMaster.FindAsync(id);
        if (existing == null) return null;

        _context.Entry(existing).CurrentValues.SetValues(formulamaster);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var formulamaster = await _context.FormulaMaster.FindAsync(id);
        if (formulamaster == null)
            return false;

        _context.FormulaMaster.Remove(formulamaster);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<FormulaMaster?> GetByUsernameAsync(string Final_Product)
    {
        return await _context.FormulaMaster.FirstOrDefaultAsync(e => e.Final_Product == Final_Product);
    }

    public IQueryable<FormulaMaster> Query() =>
        _context
            .FormulaMaster.Include(e => e.FinalProduct) // Ensure Role is loaded
            .Select(x => new FormulaMaster
            {
                Id = x.Id, // Add if needed            
               Final_Product = x.FinalProduct!.Final_Product,

                 IsActive = x.IsActive,
            });
}
