using Api.Application.DTOs;
using Api.Application.Interfaces;
using AutoMapper;
using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Api.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Services;

public class FproductListService(IFproductListRepository _repository, IMapper _mapper, AppDbContext _context) : IFproductListService
{
    private static readonly string[] _excludedSearchProperties = ["IsActive", "Id", "SrNo",];

    public async Task<PagedResultDto<FproductListDto>> GetAllAsync(PagedQueryDto query)
{
    var q = _context.FproductList
        .Include(x => x.FGramage)
        .Include(x => x.Colour)
        .AsQueryable();

    if (query.filter.Any(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)))
    {
        var searchTerms = query.filter
            .Where(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase))
            .Select(f => f.Value)
            .ToList();

        q = q.Where(SearchHelper.BuildGlobalSearchPredicate<FproductList>(searchTerms, _excludedSearchProperties));
    }

    var total = await q.CountAsync();

    q = SortHelper.ApplySorting(q, query.sort, s => s.Field, s => s.Dir)
        ?? q.OrderByDescending(n => n.Id);

    var skip = (query.page - 1) * query.size;

    var items = await q
        .Skip(skip)
        .Take(query.size)
        .Select(x => new FproductListDto
        {
            Id = x.Id,
            Name = x.Name,
            FGramageMasterId = x.FGramageMasterId,
            ColourMasterId = x.ColourMasterId,
            Comments = x.Comments,
            IsActive = x.IsActive,
            FGramageMasterName = x.FGramage != null ? x.FGramage.GRM : null,
            ColourMasterName = x.Colour != null ? x.Colour.Name : null
        })
        .ToListAsync();

    return new PagedResultDto<FproductListDto>
    {
        Items = items,
        TotalCount = total,
        Page = query.page,
        Size = query.size
    };
}

    public async Task<FproductListDto?> GetByIdAsync(int id)
    {
        var fproductList = await _context.FproductList.FirstOrDefaultAsync(e => e.Id == id);
        if (fproductList == null) return null;

        return new FproductListDto
        {
            Id = fproductList.Id,
            Name = fproductList.Name,
            FGramageMasterId = fproductList.FGramageMasterId,
            ColourMasterId = fproductList.ColourMasterId,
            Comments = fproductList.Comments,
            IsActive = fproductList.IsActive,
        };
    }

    public async Task<FproductListDto> CreateAsync(FproductListDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Check username exists in either table
            if (await _context.PVCproductList.AnyAsync(e => e.Name == dto.Name))
            {
                throw new ArgumentException("Username already exists");
            }

            // 2. Create Customer
            var fproductList = _mapper.Map<FproductList>(dto);
            await _repository.AddAsync(fproductList);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return _mapper.Map<FproductListDto>(fproductList);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<FproductListDto?> UpdateAsync(int id, FproductListDto dto)
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
            return _mapper.Map<FproductListDto>(existing);
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
            var fproductList = await _repository.GetByIdAsync(id);
            if (fproductList == null) return false;

            // Delete Customer
            _context.FproductList.Remove(fproductList);
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

    public async Task<FproductListDto?> UpdateStatusAsync(int id, short IsActive)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;
        existing.IsActive = IsActive;
        var updated = await _repository.UpdateAsync(id, existing);
        return updated is null ? null : _mapper.Map<FproductListDto>(updated);
    }
}
