using Api.Application.DTOs;
using Api.Application.Interfaces;
using AutoMapper;
using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Api.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Services;

public class PVCproductListService(IPVCproductListRepository _repository, IMapper _mapper, AppDbContext _context) : IPVCproductListService
{
    private static readonly string[] _excludedSearchProperties = ["IsActive", "Id", "SrNo",];

    public async Task<PagedResultDto<PVCproductListDto>> GetAllAsync(PagedQueryDto query)
    {
        var q = _repository.Query();

        // Apply global search
        if (query.filter.Any(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)))
        {
            var searchTerms = query.filter.Where(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)).Select(f => f.Value).ToList();

            q = q.Where(SearchHelper.BuildGlobalSearchPredicate<PVCproductList>(searchTerms, _excludedSearchProperties));
        }

        var total = await q.CountAsync();

        // Apply sorting
        q = SortHelper.ApplySorting(q, query.sort, s => s.Field, s => s.Dir) ?? q.OrderByDescending(n => n.Id);

        // Pagination
        var skip = (query.page - 1) * query.size;
        var items = await q.Skip(skip).Take(query.size).ToListAsync();

        return new PagedResultDto<PVCproductListDto>
        {
            Items = items.Select(_mapper.Map<PVCproductListDto>),
            TotalCount = total,
            Page = query.page,
            Size = query.size,
        };
    }

    public async Task<PVCproductListDto?> GetByIdAsync(int id)
    {
        var PVC = await _context.PVCproductList.FirstOrDefaultAsync(e => e.Id == id);
        if (PVC == null) return null;

        return new PVCproductListDto
        {
            Id = PVC.Id,
            Name = PVC.Name,
            Gramage = PVC.Gramage,
            Width = PVC.Width,
            Colour = PVC.Colour,
            Comments = PVC.Comments,
            IsActive = PVC.IsActive,
        };
    }

    public async Task<PVCproductListDto> CreateAsync(PVCproductListDto dto)
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
            var pvcproductlist = _mapper.Map<PVCproductList>(dto);
            await _repository.AddAsync(pvcproductlist);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return _mapper.Map<PVCproductListDto>(pvcproductlist);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<PVCproductListDto?> UpdateAsync(int id, PVCproductListDto dto)
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
            return _mapper.Map<PVCproductListDto>(existing);
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
            var pvcproductlist = await _repository.GetByIdAsync(id);
            if (pvcproductlist == null) return false;

            // Delete Customer
            _context.PVCproductList.Remove(pvcproductlist);
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

    public async Task<PVCproductListDto?> UpdateStatusAsync(int id, short IsActive)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;
        existing.IsActive = IsActive;
        var updated = await _repository.UpdateAsync(id, existing);
        return updated is null ? null : _mapper.Map<PVCproductListDto>(updated);
    }
}
