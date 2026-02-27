using Api.Application.DTOs;
using Api.Application.Interfaces;
using AutoMapper;
using System.Linq.Expressions;
using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Api.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Services;

public class PVCInwardService(IPVCInwardRepository _repository, IMapper _mapper, AppDbContext _context) : IPVCInwardService
{
    private static readonly string[] _excludedSearchProperties = [""];

    public async Task<PagedResultDto<PVCInwardDto>> GetAllAsync(PagedQueryDto query)
    {
        var q = _repository.Query();

        // Apply global search (including related Chemical and Supplier names)
        if (query.filter.Any(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)))
        {
            var searchTerms = query.filter.Where(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)).Select(f => f.Value).ToList();

            // base predicate for ChemicalInward properties
            var basePredicate = SearchHelper.BuildGlobalSearchPredicate<PVCInward>(searchTerms, _excludedSearchProperties);

            // build predicate for related entity name fields
            Expression<Func<PVCInward, bool>> relatedPredicate = ci => false;
            foreach (var term in searchTerms.Where(t => !string.IsNullOrWhiteSpace(t)))
            {
                Expression<Func<PVCInward, bool>> termPred = ci =>
                    (ci.Supplier != null && EF.Functions.Like(ci.Supplier.Name, "%" + term + "%")) ||
                    (ci.PVC != null && EF.Functions.Like(ci.PVC.Name, "%" + term + "%"));

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
    .Select(f => new PVCInwardDto
    {
        Id = f.Id,
        SupplierMasterId = f.SupplierMasterId,
        PVCMasterId = f.PVCMasterId,
        New_RollNo = f.New_RollNo,
        BatchNo = f.BatchNo,
        Qty_kg = f.Qty_kg,
        Qty_Mtr = f.Qty_Mtr,
        Comments = f.Comments,
        BillDate = f.BillDate,
        ReceivedDate = f.ReceivedDate,
        IsActive = f.IsActive,
        SupplierMasterName = f.Supplier != null ? f.Supplier.Name : string.Empty,
        PVCMasterName = f.PVC != null ? f.PVC.Name : string.Empty,
    })
    .ToListAsync();

        return new PagedResultDto<PVCInwardDto>
        {
            Items = items,
            TotalCount = total,
            Page = query.page,
            Size = query.size,
        };
    }

    public async Task<PVCInwardDto?> GetByIdAsync(int id)
    {
        var pvcInward = await _context.PVCInward.FirstOrDefaultAsync(e => e.Id == id);
        if (pvcInward == null) return null;

        return new PVCInwardDto
        {
            Id = pvcInward.Id,
            SupplierMasterId = pvcInward.SupplierMasterId,
            PVCMasterId = pvcInward.PVCMasterId,
            New_RollNo = pvcInward.New_RollNo,
            BatchNo = pvcInward.BatchNo,
            Qty_kg = pvcInward.Qty_kg,
            Qty_Mtr = pvcInward.Qty_Mtr,
            Comments = pvcInward.Comments,
            BillDate = pvcInward.BillDate,
            ReceivedDate = pvcInward.ReceivedDate,
            IsActive = pvcInward.IsActive,
            // ChemicalMasterName = chemicalInward.Name,
            // SupplierMasterName = chemicalInward.SupplierMaster.Name,
        };
    }

    public async Task<PVCInwardDto> CreateAsync(PVCInwardDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Check BatchNo uniqueness
            if (await _context.PVCInward.AnyAsync(e => e.BatchNo == dto.BatchNo))
            {
                throw new ArgumentException("Batch No already exists");
            }

            // 2. Create Final product
            var pvcInward = _mapper.Map<PVCInward>(dto);
            await _repository.AddAsync(pvcInward);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return _mapper.Map<PVCInwardDto>(pvcInward);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<PVCInwardDto?> UpdateAsync(int id, PVCInwardDto dto)
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
            return _mapper.Map<PVCInwardDto>(existing);
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
            var pvcInward = await _repository.GetByIdAsync(id);
            if (pvcInward == null) return false;

            // Delete PVCInward
            _context.PVCInward.Remove(pvcInward);
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

    public async Task<PVCInwardDto?> UpdateStatusAsync(int id, short IsActive)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;
        existing.IsActive = IsActive;
        var updated = await _repository.UpdateAsync(id, existing);
        return updated is null ? null : _mapper.Map<PVCInwardDto>(updated);
    }
}
