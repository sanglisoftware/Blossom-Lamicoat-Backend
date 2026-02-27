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
    var baseQuery = _context.Quality.AsNoTracking();

    // Apply search BEFORE include
    if (query.filter.Any(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)))
    {
        var searchTerms = query.filter
            .Where(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase))
            .Select(f => f.Value)
            .ToList();

        baseQuery = baseQuery
            .Where(SearchHelper.BuildGlobalSearchPredicate<Quality>(searchTerms, _excludedSearchProperties));
    }

    // COUNT without Include
    var total = await baseQuery.CountAsync();

    // Apply sorting
    baseQuery = SortHelper.ApplySorting(baseQuery, query.sort, s => s.Field, s => s.Dir)
                ?? baseQuery.OrderByDescending(n => n.Id);

    var skip = (query.page - 1) * query.size;

    // Apply pagination first, THEN join via Select
    var items = await baseQuery
        .Skip(skip)
        .Take(query.size)
        .Select(x => new QualityDto
        {
            Id = x.Id,
            Name = x.Name,
            Comments = x.Comments,
            GSMGLMMasterId = x.GSMGLMMasterId,
            ColourMasterId = x.ColourMasterId,
            IsActive = x.IsActive,
            GSMMasterName = x.GSM != null ? x.GSM.Name : null,
            ColourMasterName = x.Colour != null ? x.Colour.Name : null
        })
        .ToListAsync();

    return new PagedResultDto<QualityDto>
    {
        Items = items,
        TotalCount = total,
        Page = query.page,
        Size = query.size
    };
}

    public async Task<QualityDto?> GetByIdAsync(int id)
    {
        var quality = await _context.Quality.FirstOrDefaultAsync(e => e.Id == id);
        if (quality == null) return null;

        return new QualityDto
        {
            Id = quality.Id,
            Name = quality.Name,
            Comments = quality.Comments,
            GSMGLMMasterId = quality.GSMGLMMasterId,
            ColourMasterId = quality.ColourMasterId,
            IsActive = quality.IsActive,
            GSMMasterName = quality.GSM != null ? quality.GSM.Name : null,
            ColourMasterName = quality.Colour != null ? quality.Colour.Name : null
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
                throw new ArgumentException("Quality already exists");
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
