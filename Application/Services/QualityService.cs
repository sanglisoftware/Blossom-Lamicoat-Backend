using Api.Application.DTOs;
using Api.Application.Interfaces;
using AutoMapper;
using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Api.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Services;

public class QualityService(IQualityRepository _repository, IMapper _mapper, AppDbContext _context) : IQualityService
{
    private static readonly string[] _excludedSearchProperties = ["IsActive", "Id", "SrNo",];

    public async Task<PagedResultDto<QualityDto>> GetAllAsync(PagedQueryDto query)
    {
        var q = _repository.Query();

        // Apply global search
        if (query.filter.Any(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)))
        {
            var searchTerms = query.filter.Where(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)).Select(f => f.Value).ToList();

            q = q.Where(SearchHelper.BuildGlobalSearchPredicate<Quality>(searchTerms, _excludedSearchProperties));
        }

        var total = await q.CountAsync();

        // Apply sorting
        q = SortHelper.ApplySorting(q, query.sort, s => s.Field, s => s.Dir) ?? q.OrderByDescending(n => n.Id);

        // Pagination
        var skip = (query.page - 1) * query.size;
        var items = await q.Skip(skip).Take(query.size).ToListAsync();

        return new PagedResultDto<QualityDto>
        {
            Items = items.Select(_mapper.Map<QualityDto>),
            TotalCount = total,
            Page = query.page,
            Size = query.size,
        };
    }

    public async Task<QualityDto?> GetByIdAsync(int id)
    {
        var Qua = await _context.Quality.FirstOrDefaultAsync(e => e.Id == id);
        if (Qua == null) return null;

        return new QualityDto
        {
            Id = Qua.Id,
            Name = Qua.Name,
            Comments = Qua.Comments,
            GSM_GLM = Qua.GSM_GLM,
            Colour = Qua.Colour,
            IsActive = Qua.IsActive,
        };
    }

    public async Task<QualityDto> CreateAsync(QualityDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Check username exists in either table
            if (await _context.Quality.AnyAsync(e => e.Name == dto.Name))
            {
                throw new ArgumentException("Username already exists");
            }

            // 2. Create Customer
            var quality = _mapper.Map<Quality>(dto);
            await _repository.AddAsync(quality);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return _mapper.Map<QualityDto>(quality);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<QualityDto?> UpdateAsync(int id, QualityDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Update Customer
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            _mapper.Map(dto, existing);
            existing.Id = id;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return _mapper.Map<QualityDto>(existing);
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
            var quality = await _repository.GetByIdAsync(id);
            if (quality == null) return false;

            // Delete Customer
            _context.Quality.Remove(quality);
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

    public async Task<QualityDto?> UpdateStatusAsync(int id, short IsActive)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;
        existing.IsActive = IsActive;
        var updated = await _repository.UpdateAsync(id, existing);
        return updated is null ? null : _mapper.Map<QualityDto>(updated);
    }
}
