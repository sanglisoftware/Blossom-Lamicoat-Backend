using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class GradeRepository(AppDbContext _context) : IGradeRepository
{
    public async Task<IEnumerable<Grade>> GetAllAsync() => await _context.Grade.ToListAsync();

    public async Task<Grade?> GetByIdAsync(int id) => await _context.Grade.FindAsync(id);

    public async Task<Grade> AddAsync(Grade grade)
    {
        await _context.Grade.AddAsync(grade);
        return grade;
    }

    public async Task<Grade?> UpdateAsync(int id, Grade grade)
    {
        var existing = await _context.Grade.FindAsync(id);
        if (existing == null) return null;

        _context.Entry(existing).CurrentValues.SetValues(grade);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var grade = await _context.Grade.FindAsync(id);
        if (grade == null)
            return false;

        _context.Grade.Remove(grade);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Grade?> GetByNameAsync(string name)
    {
        return await _context.Grade.FirstOrDefaultAsync(e => e.Name == name);
    }

    public IQueryable<Grade> Query() =>
        _context.Grade
            .Select(x => new Grade
            {
                Id = x.Id, // Add if needed
                Name = x.Name,
                IsActive = x.IsActive,
            });
}
