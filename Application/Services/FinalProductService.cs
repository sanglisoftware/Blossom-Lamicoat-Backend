using Api.Application.DTOs;
using Api.Application.Interfaces;
using AutoMapper;
using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Api.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Services;

public class FinalProductService(IFinalProductRepository _repository, IMapper _mapper, AppDbContext _context) : IFinalProductService
{
    private static readonly string[] _excludedSearchProperties = ["IsActive", "Id", "SrNo",];

    public async Task<PagedResultDto<FinalProductDto>> GetAllAsync(PagedQueryDto query)
    {
        var q = _repository.Query();

        // Apply global search
        if (query.filter.Any(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)))
        {
            var searchTerms = query.filter.Where(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)).Select(f => f.Value).ToList();

            q = q.Where(SearchHelper.BuildGlobalSearchPredicate<FinalProduct>(searchTerms, _excludedSearchProperties));
        }

        var total = await q.CountAsync();

        // Apply sorting
        q = SortHelper.ApplySorting(q, query.sort, s => s.Field, s => s.Dir) ?? q.OrderByDescending(n => n.Id);

        // Pagination
        var skip = (query.page - 1) * query.size;
        var items = await q.Skip(skip).Take(query.size).ToListAsync();

        return new PagedResultDto<FinalProductDto>
        {
            Items = items.Select(_mapper.Map<FinalProductDto>),
            TotalCount = total,
            Page = query.page,
            Size = query.size,
        };
    }

    public async Task<FinalProductDto?> GetByIdAsync(int id)
    {
        var FinalP = await _context.FinalProduct.FirstOrDefaultAsync(e => e.Id == id);
        if (FinalP == null) return null;

        return new FinalProductDto
        {
            Id = FinalP.Id,
            Final_Product = FinalP.Final_Product,
            IsActive = FinalP.IsActive,
        };
    }

    public async Task<FinalProductDto> CreateAsync(FinalProductDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Check FinalProduct exists in either table
            if (await _context.FinalProduct.AnyAsync(e => e.Final_Product == dto.Final_Product))
            {
                throw new ArgumentException("Final Product already exists");
            }

            // 2. Create FinalProduct
            var finalproduct = _mapper.Map<FinalProduct>(dto);
            await _repository.AddAsync(finalproduct);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return _mapper.Map<FinalProductDto>(finalproduct);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<FinalProductDto?> UpdateAsync(int id, FinalProductDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Update FinalProduct
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            _mapper.Map(dto, existing);
            existing.Id = id;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return _mapper.Map<FinalProductDto>(existing);
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
            var finalproduct = await _repository.GetByIdAsync(id);
            if (finalproduct == null) return false;

            // Delete FinalProduct
            _context.FinalProduct.Remove(finalproduct);
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

    public async Task<FinalProductDto?> UpdateStatusAsync(int id, short IsActive)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;
        existing.IsActive = IsActive;
        var updated = await _repository.UpdateAsync(id, existing);
        return updated is null ? null : _mapper.Map<FinalProductDto>(updated);
    }
}
