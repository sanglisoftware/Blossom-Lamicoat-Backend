using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class SupplierRepository(AppDbContext _context) : ISupplierRepository
{
    public async Task<IEnumerable<Supplier>> GetAllAsync() => await _context.Supplier.ToListAsync();

    public async Task<Supplier?> GetByIdAsync(int id) => await _context.Supplier.FindAsync(id);

    public async Task<Supplier> AddAsync(Supplier supplier)
    {
        await _context.Supplier.AddAsync(supplier);
        return supplier;
    }

    public async Task<Supplier?> UpdateAsync(int id, Supplier supplier)
    {
        var existing = await _context.Supplier.FindAsync(id);
        if (existing == null) return null;

        _context.Entry(existing).CurrentValues.SetValues(supplier);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var supplier = await _context.Supplier.FindAsync(id);
        if (supplier == null)
            return false;

        _context.Supplier.Remove(supplier);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Supplier?> GetByNameAsync(string name)
    {
        return await _context.Supplier.FirstOrDefaultAsync(e => e.Name == name);
    }

    public IQueryable<Supplier> Query() =>
        _context.Supplier
            .Select(x => new Supplier
            {
                Id = x.Id, // Add if needed
                Name = x.Name,
                Address = x.Address,
                Mobile_No = x.Mobile_No,
                Pan = x.Pan,
                GST_No = x.GST_No,
                IsActive = x.IsActive,
            });
}
