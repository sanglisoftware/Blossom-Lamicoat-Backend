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
        var q = _repository.Query();

        // Apply global search
        if (query.filter.Any(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)))
        {
            var searchTerms = query.filter.Where(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)).Select(f => f.Value).ToList();

            q = q.Where(SearchHelper.BuildGlobalSearchPredicate<FproductList>(searchTerms, _excludedSearchProperties));
        }

        var total = await q.CountAsync();

        // Apply sorting
        q = SortHelper.ApplySorting(q, query.sort, s => s.Field, s => s.Dir) ?? q.OrderByDescending(n => n.Id);

        // Pagination
        var skip = (query.page - 1) * query.size;
        var items = await q.Skip(skip).Take(query.size).ToListAsync();

        return new PagedResultDto<FproductListDto>
        {
            Items = items.Select(_mapper.Map<FproductListDto>),
            TotalCount = total,
            Page = query.page,
            Size = query.size,
        };
    }

    public async Task<FproductListDto?> GetByIdAsync(int id)
    {
        var Fproduct = await _context.FproductList.FirstOrDefaultAsync(e => e.Id == id);
        if (Fproduct == null) return null;

        return new FproductListDto
        {
            Id = Fproduct.Id,
            Name = Fproduct.Name,
            GRM = Fproduct.GRM,
            Colour = Fproduct.Colour,
            Comments = Fproduct.Comments,
            IsActive = Fproduct.IsActive,
        };
    }

    public async Task<FproductListDto> CreateAsync(FproductListDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Check username exists in either table
            if (await _context.FproductList.AnyAsync(e => e.Name == dto.Name))
            {
                throw new ArgumentException("Username already exists");
            }

            // 2. Create Customer
            var fproductlist = _mapper.Map<FproductList>(dto);
            await _repository.AddAsync(fproductlist);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return _mapper.Map<FproductListDto>(fproductlist);
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
            var fproductlist = await _repository.GetByIdAsync(id);
            if (fproductlist == null) return false;

            // Delete Customer
            _context.FproductList.Remove(fproductlist);
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
