using Api.Application.DTOs;
using Api.Application.Interfaces;
using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Api.Infrastructure.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Services;

public class UnitOfMeasurementService(
    IUnitOfMeasurementRepository _repository,
    IMapper _mapper,
    AppDbContext _context) : IUnitOfMeasurementService
{
    private static readonly string[] _excludedSearchProperties = ["IsActive", "Id"];

    public async Task<PagedResultDto<UnitOfMeasurementDto>> GetAllAsync(PagedQueryDto query)
    {
        var q = _repository.Query();

        if (query.filter.Any(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)))
        {
            var searchTerms = query.filter
                .Where(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase))
                .Select(f => f.Value)
                .ToList();

            q = q.Where(SearchHelper.BuildGlobalSearchPredicate<UnitOfMeasurement>(searchTerms, _excludedSearchProperties));
        }

        var total = await q.CountAsync();
        q = SortHelper.ApplySorting(q, query.sort, s => s.Field, s => s.Dir) ?? q.OrderByDescending(x => x.Id);

        var skip = (query.page - 1) * query.size;
        var items = await q.Skip(skip).Take(query.size).ToListAsync();

        return new PagedResultDto<UnitOfMeasurementDto>
        {
            Items = items.Select(_mapper.Map<UnitOfMeasurementDto>),
            TotalCount = total,
            Page = query.page,
            Size = query.size,
        };
    }

    public async Task<UnitOfMeasurementDto?> GetByIdAsync(int id)
    {
        var unit = await _repository.GetByIdAsync(id);
        return unit == null ? null : _mapper.Map<UnitOfMeasurementDto>(unit);
    }

    public async Task<UnitOfMeasurementDto> CreateAsync(UnitOfMeasurementDto dto)
    {
        var name = dto.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Unit name is required");
        }

        if (await _context.UnitOfMeasurements.AnyAsync(x => x.Name == name))
        {
            throw new ArgumentException("Unit already exists");
        }

        var unit = _mapper.Map<UnitOfMeasurement>(dto);
        unit.Name = name;
        unit.IsActive ??= 1;

        await _repository.AddAsync(unit);
        await _context.SaveChangesAsync();
        return _mapper.Map<UnitOfMeasurementDto>(unit);
    }

    public async Task<UnitOfMeasurementDto?> UpdateAsync(int id, UnitOfMeasurementDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;

        var name = dto.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Unit name is required");
        }

        if (await _context.UnitOfMeasurements.AnyAsync(x => x.Id != id && x.Name == name))
        {
            throw new ArgumentException("Unit already exists");
        }

        existing.Name = name;
        existing.IsActive = dto.IsActive ?? existing.IsActive ?? 1;

        var updated = await _repository.UpdateAsync(id, existing);
        return updated == null ? null : _mapper.Map<UnitOfMeasurementDto>(updated);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (await _context.ChemicalInward.AnyAsync(x => x.UnitOfMeasurementId == id))
        {
            throw new InvalidOperationException("Unit is used in chemical inward records");
        }

        return await _repository.DeleteAsync(id);
    }

    public async Task<UnitOfMeasurementDto?> UpdateStatusAsync(int id, short isActive)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;

        existing.IsActive = isActive;
        var updated = await _repository.UpdateAsync(id, existing);
        return updated == null ? null : _mapper.Map<UnitOfMeasurementDto>(updated);
    }
}
