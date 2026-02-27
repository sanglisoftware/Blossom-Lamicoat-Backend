using Api.Application.DTOs;
using Api.Application.Interfaces;
using AutoMapper;
using System.Linq.Expressions;
using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Api.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Azure;

namespace Api.Application.Services;

public class FabricInwardService(IFabricInwardRepository _repository, IMapper _mapper, AppDbContext _context) : IFabricInwardService
{
    private static readonly string[] _excludedSearchProperties = [""];

    public async Task<PagedResultDto<FabricInwardDto>> GetAllAsync(PagedQueryDto query)
    {
        var q = _repository.Query();

        // Apply global search (including related Chemical and Supplier names)
        if (query.filter.Any(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)))
        {
            var searchTerms = query.filter.Where(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)).Select(f => f.Value).ToList();

            // base predicate for ChemicalInward properties
            var basePredicate = SearchHelper.BuildGlobalSearchPredicate<FabricInward>(searchTerms, _excludedSearchProperties);

            // build predicate for related entity name fields
            Expression<Func<FabricInward, bool>> relatedPredicate = ci => false;
            foreach (var term in searchTerms.Where(t => !string.IsNullOrWhiteSpace(t)))
            {
                Expression<Func<FabricInward, bool>> termPred = ci =>
                    (ci.Fabric != null && EF.Functions.Like(ci.Fabric.Name, "%" + term + "%")) ||
                    (ci.Supplier != null && EF.Functions.Like(ci.Supplier.Name, "%" + term + "%"));

                relatedPredicate = SearchHelper.CombineOr(relatedPredicate, termPred);
            }

            // combine base predicate and related predicate with OR
            var combined = SearchHelper.CombineOr(basePredicate, relatedPredicate);
            q = q.Where(combined);
        }

        var total = await q.CountAsync();

        // Apply sorting
        q = SortHelper.ApplySorting(q, query.sort, s => s.Field, s => s.Dir) ?? q.OrderByDescending(n => n.Id);

        // Pagination
        var skip = (query.page - 1) * query.size;
        //var items = await q.Skip(skip).Take(query.size).ToListAsync();
        var items = await q
    .Skip(skip)
    .Take(query.size)
    .Select(f => new FabricInwardDto
    {
        Id = f.Id,
        SupplierMasterId = f.SupplierMasterId,
        FabricMasterId = f.FabricMasterId,
        BatchNo = f.BatchNo,
        QtyMTR = f.QtyMTR,
        Comments = f.Comments,
        IsActive = f.IsActive,
        SupplierMasterName = f.Supplier != null ? f.Supplier.Name : string.Empty,
        FabricMasterName = f.Fabric != null ? f.Fabric.Name : string.Empty,
    })
    .ToListAsync();

        return new PagedResultDto<FabricInwardDto>
        {
            Items = items,
            TotalCount = total,
            Page = query.page,
            Size = query.size,
        };
    }

    public async Task<FabricInwardDto?> GetByIdAsync(int id)
    {
        var fabricInward = await _context.FabricInward
     .Include(f => f.Supplier)
     .Include(f => f.Fabric)
     .FirstOrDefaultAsync(e => e.Id == id);
        if (fabricInward == null) return null;

        return new FabricInwardDto
        {
            Id = fabricInward.Id,
            SupplierMasterId = fabricInward.SupplierMasterId,
            FabricMasterId = fabricInward.FabricMasterId,
            BatchNo = fabricInward.BatchNo,
            QtyMTR = fabricInward.QtyMTR,
            Comments = fabricInward.Comments,
            IsActive = fabricInward.IsActive,
            // ChemicalMasterName = chemicalInward.Name,
            // SupplierMasterName = fabricInward.Supplier.Name,
            // FabricMasterName = fabricInward.Fabric.Name,
        };
    }

    public async Task<FabricInwardDto> CreateAsync(FabricInwardDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Check GRM exists in either table
            if (await _context.FabricInward.AnyAsync(e => e.BatchNo == dto.BatchNo))
            {
                throw new ArgumentException("Batch No already exists");
            }

            // 2. Create Final product
            var fabricInward = _mapper.Map<FabricInward>(dto);
            await _repository.AddAsync(fabricInward);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return _mapper.Map<FabricInwardDto>(fabricInward);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<FabricInwardDto?> UpdateAsync(int id, FabricInwardDto dto)
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
            return _mapper.Map<FabricInwardDto>(existing);
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
            var fabricInward = await _repository.GetByIdAsync(id);
            if (fabricInward == null) return false;

            // Delete FabricInward
            _context.FabricInward.Remove(fabricInward);
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

    public async Task<FabricInwardDto?> UpdateStatusAsync(int id, short IsActive)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;
        existing.IsActive = IsActive;
        var updated = await _repository.UpdateAsync(id, existing);
        return updated is null ? null : _mapper.Map<FabricInwardDto>(updated);
    }
}


