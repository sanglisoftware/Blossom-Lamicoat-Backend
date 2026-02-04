using Api.Application.DTOs;
using Api.Application.Interfaces;
using AutoMapper;
using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Api.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Services;

public class GSMService(IGSMRepository _repository, IMapper _mapper, AppDbContext _context) : IGSMService
{
    private static readonly string[] _excludedSearchProperties = ["IsActive", "Id", "SrNo",];

    public async Task<PagedResultDto<GSMDto>> GetAllAsync(PagedQueryDto query)
    {
        var q = _repository.Query();

        // Apply global search
        if (query.filter.Any(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)))
        {
            var searchTerms = query.filter.Where(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)).Select(f => f.Value).ToList();

            q = q.Where(SearchHelper.BuildGlobalSearchPredicate<GSM>(searchTerms, _excludedSearchProperties));
        }

        var total = await q.CountAsync();

        // Apply sorting
        q = SortHelper.ApplySorting(q, query.sort, s => s.Field, s => s.Dir) ?? q.OrderByDescending(n => n.Id);

        // Pagination
        var skip = (query.page - 1) * query.size;
        var items = await q.Skip(skip).Take(query.size).ToListAsync();

        return new PagedResultDto<GSMDto>
        {
            Items = items.Select(_mapper.Map<GSMDto>),
            TotalCount = total,
            Page = query.page,
            Size = query.size,
        };
    }

    public async Task<GSMDto?> GetByIdAsync(int id)
    {
        var GS = await _context.GSM.FirstOrDefaultAsync(e => e.Id == id);
        if (GS == null) return null;

        return new GSMDto
        {
            Id = GS.Id,
            Name = GS.Name,
            IsActive = GS.IsActive,
        };
    }

    public async Task<GSMDto> CreateAsync(GSMDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Check GRM exists in either table
            if (await _context.GSM.AnyAsync(e => e.Name == dto.Name))
            {
                throw new ArgumentException("GRM already exists");
            }

            // 2. Create GRM
            var gsm = _mapper.Map<GSM>(dto);
            await _repository.AddAsync(gsm);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return _mapper.Map<GSMDto>(gsm);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<GSMDto?> UpdateAsync(int id, GSMDto dto)
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
            return _mapper.Map<GSMDto>(existing);
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
            var gsm = await _repository.GetByIdAsync(id);
            if (gsm == null) return false;

            // Delete Gramage
            _context.GSM.Remove(gsm);
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

    public async Task<GSMDto?> UpdateStatusAsync(int id, short IsActive)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;
        existing.IsActive = IsActive;
        var updated = await _repository.UpdateAsync(id, existing);
        return updated is null ? null : _mapper.Map<GSMDto>(updated);
    }
}
