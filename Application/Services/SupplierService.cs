using Api.Application.DTOs;
using Api.Application.Interfaces;
using AutoMapper;
using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Api.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Services;

public class SupplierService(ISupplierRepository _repository, IMapper _mapper, AppDbContext _context) : ISupplierService
{
    private static readonly string[] _excludedSearchProperties = ["IsActive", "Id", "SrNo",];

    public async Task<PagedResultDto<SupplierDto>> GetAllAsync(PagedQueryDto query)
    {
        var q = _repository.Query();

        // Apply global search
        if (query.filter.Any(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)))
        {
            var searchTerms = query.filter.Where(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)).Select(f => f.Value).ToList();

            q = q.Where(SearchHelper.BuildGlobalSearchPredicate<Supplier>(searchTerms, _excludedSearchProperties));
        }

        var total = await q.CountAsync();

        // Apply sorting
        q = SortHelper.ApplySorting(q, query.sort, s => s.Field, s => s.Dir) ?? q.OrderByDescending(n => n.Id);

        // Pagination
        var skip = (query.page - 1) * query.size;
        var items = await q.Skip(skip).Take(query.size).ToListAsync();

        return new PagedResultDto<SupplierDto>
        {
            Items = items.Select(_mapper.Map<SupplierDto>),
            TotalCount = total,
            Page = query.page,
            Size = query.size,
        };
    }

    public async Task<SupplierDto?> GetByIdAsync(int id)
    {
        var Supp = await _context.Supplier.FirstOrDefaultAsync(e => e.Id == id);
        if (Supp == null) return null;

        return new SupplierDto
        {
            Id = Supp.Id,
            Name = Supp.Name,
            Address = Supp.Address,
            Mobile_No = Supp.Mobile_No,
            Pan = Supp.Pan,
            GST_No = Supp.GST_No,
            IsActive = Supp.IsActive,
        };
    }

    public async Task<SupplierDto> CreateAsync(SupplierDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Check username exists in either table
            if (await _context.Supplier.AnyAsync(e => e.Name == dto.Name))
            {
                throw new ArgumentException("Username already exists");
            }

            // 2. Create Supplier
            var supplier = _mapper.Map<Supplier>(dto);
            await _repository.AddAsync(supplier);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return _mapper.Map<SupplierDto>(supplier);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<SupplierDto?> UpdateAsync(int id, SupplierDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Update Supplier
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            _mapper.Map(dto, existing);
            existing.Id = id;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return _mapper.Map<SupplierDto>(existing);
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
            var supplier = await _repository.GetByIdAsync(id);
            if (supplier == null) return false;

            // Delete Supplier
            _context.Supplier.Remove(supplier);
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

    public async Task<SupplierDto?> UpdateStatusAsync(int id, short IsActive)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;
        existing.IsActive = IsActive;
        var updated = await _repository.UpdateAsync(id, existing);
        return updated is null ? null : _mapper.Map<SupplierDto>(updated);
    }
}
