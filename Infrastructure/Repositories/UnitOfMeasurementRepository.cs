using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class UnitOfMeasurementRepository(AppDbContext _context) : IUnitOfMeasurementRepository
{
    public IQueryable<UnitOfMeasurement> Query() =>
        _context.UnitOfMeasurements.Select(x => new UnitOfMeasurement
        {
            Id = x.Id,
            Name = x.Name,
            IsActive = x.IsActive,
        });

    public async Task<UnitOfMeasurement?> GetByIdAsync(int id) =>
        await _context.UnitOfMeasurements.FindAsync(id);

    public async Task<UnitOfMeasurement> AddAsync(UnitOfMeasurement unitOfMeasurement)
    {
        await _context.UnitOfMeasurements.AddAsync(unitOfMeasurement);
        return unitOfMeasurement;
    }

    public async Task<UnitOfMeasurement?> UpdateAsync(int id, UnitOfMeasurement unitOfMeasurement)
    {
        var existing = await _context.UnitOfMeasurements.FindAsync(id);
        if (existing == null) return null;

        _context.Entry(existing).CurrentValues.SetValues(unitOfMeasurement);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.UnitOfMeasurements.FindAsync(id);
        if (existing == null) return false;

        _context.UnitOfMeasurements.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}
