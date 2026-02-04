using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class GSMRepository(AppDbContext _context) : IGSMRepository
{
    public async Task<IEnumerable<GSM>> GetAllAsync() => await _context.GSM.ToListAsync();

    public async Task<GSM?> GetByIdAsync(int id) => await _context.GSM.FindAsync(id);

    public async Task<GSM> AddAsync(GSM gsm)
    {
        await _context.GSM.AddAsync(gsm);
        return gsm;
    }

    public async Task<GSM?> UpdateAsync(int id, GSM gsm)
    {
        var existing = await _context.GSM.FindAsync(id);
        if (existing == null) return null;

        _context.Entry(existing).CurrentValues.SetValues(gsm);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var gsm = await _context.GSM.FindAsync(id);
        if (gsm == null)
            return false;

        _context.GSM.Remove(gsm);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<GSM?> GetByNameAsync(string name)
    {
        return await _context.GSM.FirstOrDefaultAsync(e => e.Name == name);
    }

    public IQueryable<GSM> Query() =>
        _context.GSM
            .Select(x => new GSM
            {
                Id = x.Id, // Add if needed
                Name = x.Name,
                IsActive = x.IsActive,
            });
}
