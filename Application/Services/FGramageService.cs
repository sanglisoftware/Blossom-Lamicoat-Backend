using Api.Application.DTOs;
using Api.Application.Interfaces;
using AutoMapper;
using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Api.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Services;

public class FGramageService(IFGramageRepository _repository, IMapper _mapper, AppDbContext _context) : IFGramageService
{
    private static readonly string[] _excludedSearchProperties = ["IsActive", "Id", "SrNo",];

  public async Task<PagedResultDto<FGramageDto>> GetAllAsync(PagedQueryDto query)
{
    var q = _repository.Query();

    // 🔥 SAFE NULL CHECKS
    if (query.filter != null && query.filter.Any(f =>
        f.Type != null &&
        f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)))
    {
        var searchTerms = query.filter
            .Where(f => f.Type != null &&
                        f.Type.Equals("like", StringComparison.OrdinalIgnoreCase))
            .Select(f => f.Value)
            .ToList();

        q = q.Where(SearchHelper.BuildGlobalSearchPredicate<FGramage>(
            searchTerms,
            _excludedSearchProperties));
    }

    var total = await q.CountAsync();

    // SAFE SORTING
    if (query.sort != null && query.sort.Any())
    {
        q = SortHelper.ApplySorting(q, query.sort, s => s.Field, s => s.Dir)
            ?? q.OrderByDescending(n => n.Id);
    }
    else
    {
        q = q.OrderByDescending(n => n.Id);
    }

    // SAFE PAGINATION
    var page = query.page <= 0 ? 1 : query.page;
    var size = query.size <= 0 ? 10 : query.size;

    var skip = (page - 1) * size;

    var items = await q.Skip(skip).Take(size).ToListAsync();

    return new PagedResultDto<FGramageDto>
    {
        Items = items.Select(_mapper.Map<FGramageDto>),
        TotalCount = total,
        Page = page,
        Size = size,
    };
}

    public async Task<FGramageDto?> GetByIdAsync(int id)
    {
        var FGrama = await _context.FGramage.FirstOrDefaultAsync(e => e.Id == id);
        if (FGrama == null) return null;

        return new FGramageDto
        {
            Id = FGrama.Id,
            GRM = FGrama.GRM,
            IsActive = FGrama.IsActive,
        };
    }

    public async Task<FGramageDto> CreateAsync(FGramageDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Check GRM exists in either table
            if (await _context.FGramage.AnyAsync(e => e.GRM == dto.GRM))
            {
                throw new ArgumentException("GRM already exists");
            }

            // 2. Create GRM
            var fgramage = _mapper.Map<FGramage>(dto);
            await _repository.AddAsync(fgramage);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return _mapper.Map<FGramageDto>(fgramage);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<FGramageDto?> UpdateAsync(int id, FGramageDto dto)
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
            return _mapper.Map<FGramageDto>(existing);
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
            var fgramage = await _repository.GetByIdAsync(id);
            if (fgramage == null) return false;

            // Delete Gramage
            _context.FGramage.Remove(fgramage);
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

    public async Task<FGramageDto?> UpdateStatusAsync(int id, short IsActive)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;
        existing.IsActive = IsActive;
        var updated = await _repository.UpdateAsync(id, existing);
        return updated is null ? null : _mapper.Map<FGramageDto>(updated);
    }
}
