using Api.Application.DTOs;
using Api.Application.Interfaces;
using AutoMapper;
using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Api.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Services;

public class WidthService(IWidthRepository _repository, IMapper _mapper, AppDbContext _context) : IWidthService
{
    private static readonly string[] _excludedSearchProperties = ["IsActive", "Id", "SrNo",];

    public async Task<PagedResultDto<WidthDto>> GetAllAsync(PagedQueryDto query)
    {
        var q = _repository.Query();

        // Apply global search
        if (query.filter.Any(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)))
        {
            var searchTerms = query.filter.Where(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)).Select(f => f.Value).ToList();

            q = q.Where(SearchHelper.BuildGlobalSearchPredicate<Width>(searchTerms, _excludedSearchProperties));
        }

        var total = await q.CountAsync();

        // Apply sorting
        q = SortHelper.ApplySorting(q, query.sort, s => s.Field, s => s.Dir) ?? q.OrderByDescending(n => n.Id);

        // Pagination
        var skip = (query.page - 1) * query.size;
        var items = await q.Skip(skip).Take(query.size).ToListAsync();

        return new PagedResultDto<WidthDto>
        {
            Items = items.Select(_mapper.Map<WidthDto>),
            TotalCount = total,
            Page = query.page,
            Size = query.size,
        };
    }

    public async Task<WidthDto?> GetByIdAsync(int id)
    {
        var Wid = await _context.Width.FirstOrDefaultAsync(e => e.Id == id);
        if (Wid == null) return null;

        return new WidthDto
        {
            Id = Wid.Id,
            GRM = Wid.GRM,
            IsActive = Wid.IsActive,
        };
    }

    public async Task<WidthDto> CreateAsync(WidthDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Check Width exists in either table
            if (await _context.Width.AnyAsync(e => e.GRM == dto.GRM))
            {
                throw new ArgumentException("GRM already exists");
            }

            // 2. Create Width
            var width = _mapper.Map<Width>(dto);
            await _repository.AddAsync(width);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return _mapper.Map<WidthDto>(width);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<WidthDto?> UpdateAsync(int id, WidthDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Update Width
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            _mapper.Map(dto, existing);
            existing.Id = id;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return _mapper.Map<WidthDto>(existing);
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
            var width = await _repository.GetByIdAsync(id);
            if (width == null) return false;

            // Delete width
            _context.Width.Remove(width);
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

    public async Task<WidthDto?> UpdateStatusAsync(int id, short IsActive)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;
        existing.IsActive = IsActive;
        var updated = await _repository.UpdateAsync(id, existing);
        return updated is null ? null : _mapper.Map<WidthDto>(updated);
    }
}
