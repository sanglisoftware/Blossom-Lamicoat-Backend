using Api.Application.DTOs;
using Api.Application.Interfaces;
using AutoMapper;
using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Api.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Services;

public class FormulaMasterService(IFormulaMasterRepository _repository, IMapper _mapper, AppDbContext _context) : IFormulaMasterService
{
    private static readonly string[] _excludedSearchProperties = ["IsActive", "Id", "SrNo",];

    public async Task<PagedResultDto<FormulaMasterDto>> GetAllAsync(PagedQueryDto query)
    {
        var q = _repository.Query();

        // Apply global search
        if (query.filter.Any(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)))
        {
            var searchTerms = query.filter.Where(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)).Select(f => f.Value).ToList();

            q = q.Where(SearchHelper.BuildGlobalSearchPredicate<FormulaMaster>(searchTerms, _excludedSearchProperties));
        }

        var total = await q.CountAsync();

        // Apply sorting
        q = SortHelper.ApplySorting(q, query.sort, s => s.Field, s => s.Dir) ?? q.OrderByDescending(n => n.Id);

        // Pagination
        var skip = (query.page - 1) * query.size;
        var items = await q.Skip(skip).Take(query.size).ToListAsync();

        return new PagedResultDto<FormulaMasterDto>
        {
            Items = items.Select(_mapper.Map<FormulaMasterDto>),
            TotalCount = total,
            Page = query.page,
            Size = query.size,
        };
    }

    public async Task<FormulaMasterDto?> GetByIdAsync(int id)
    {
        var formula = await _context.FormulaMaster.FirstOrDefaultAsync(e => e.Id == id);
        if (formula == null) return null;

        return new FormulaMasterDto
        {
            Id = formula.Id,
            FinalProductId = formula.FinalProductId,
            IsActive = formula.IsActive,
        };
    }

    public async Task<FormulaMasterDto> CreateAsync(FormulaMasterDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Check GRM exists in either table
            if (await _context.FormulaMaster.AnyAsync(e => e.FinalProductId == dto.FinalProductId))
            {
                throw new ArgumentException("Final Product already exists");
            }

            // 2. Create Final product
            var formulamaster = _mapper.Map<FormulaMaster>(dto);
            await _repository.AddAsync(formulamaster);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return _mapper.Map<FormulaMasterDto>(formulamaster);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<FormulaMasterDto?> UpdateAsync(int id, FormulaMasterDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Update Gramage
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            _mapper.Map(dto, existing);
            existing.Id = id;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return _mapper.Map<FormulaMasterDto>(existing);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var formulamaster = await _repository.GetByIdAsync(id);
            if (formulamaster == null) return false;

            // Delete Gramage
            _context.FormulaMaster.Remove(formulamaster);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<FormulaMasterDto?> UpdateStatusAsync(int id, short IsActive)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;
        existing.IsActive = IsActive;
        var updated = await _repository.UpdateAsync(id, existing);
        return updated is null ? null : _mapper.Map<FormulaMasterDto>(updated);
    }
}
