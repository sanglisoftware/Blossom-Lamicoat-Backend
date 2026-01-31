using Api.Application.DTOs;
using Api.Application.Interfaces;
using AutoMapper;
using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Api.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Services;

public class GradeService(IGradeRepository _repository, IMapper _mapper, AppDbContext _context) : IGradeService
{
    private static readonly string[] _excludedSearchProperties = ["IsActive", "Id", "SrNo",];

    public async Task<PagedResultDto<GradeDto>> GetAllAsync(PagedQueryDto query)
    {
        var q = _repository.Query();

        // Apply global search
        if (query.filter.Any(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)))
        {
            var searchTerms = query.filter.Where(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)).Select(f => f.Value).ToList();

            q = q.Where(SearchHelper.BuildGlobalSearchPredicate<Grade>(searchTerms, _excludedSearchProperties));
        }

        var total = await q.CountAsync();

        // Apply sorting
        q = SortHelper.ApplySorting(q, query.sort, s => s.Field, s => s.Dir) ?? q.OrderByDescending(n => n.Id);

        // Pagination
        var skip = (query.page - 1) * query.size;
        var items = await q.Skip(skip).Take(query.size).ToListAsync();

        return new PagedResultDto<GradeDto>
        {
            Items = items.Select(_mapper.Map<GradeDto>),
            TotalCount = total,
            Page = query.page,
            Size = query.size,
        };
    }

    public async Task<GradeDto?> GetByIdAsync(int id)
    {
        var gra = await _context.Grade.FirstOrDefaultAsync(e => e.Id == id);
        if (gra == null) return null;

        return new GradeDto
        {
            Id = gra.Id,
            Name = gra.Name,
            IsActive = gra.IsActive,
        };
    }

    public async Task<GradeDto> CreateAsync(GradeDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Check username exists in either table
            if (await _context.Grade.AnyAsync(e => e.Name == dto.Name))
            {
                throw new ArgumentException("Username already exists");
            }

            // 2. Create Grade
            var grade = _mapper.Map<Grade>(dto);
            await _repository.AddAsync(grade);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return _mapper.Map<GradeDto>(grade);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<GradeDto?> UpdateAsync(int id, GradeDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Update Grade
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            _mapper.Map(dto, existing);
            existing.Id = id;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return _mapper.Map<GradeDto>(existing);
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
            var grade = await _repository.GetByIdAsync(id);
            if (grade == null) return false;

            // Delete Grade
            _context.Grade.Remove(grade);
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

    public async Task<GradeDto?> UpdateStatusAsync(int id, short IsActive)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;
        existing.IsActive = IsActive;
        var updated = await _repository.UpdateAsync(id, existing);
        return updated is null ? null : _mapper.Map<GradeDto>(updated);
    }
}
