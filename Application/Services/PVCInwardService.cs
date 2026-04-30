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
    private static readonly Dictionary<string, string> _sortFieldMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["id"] = nameof(PVCInwardDto.Id),
        ["supplierMasterName"] = nameof(PVCInwardDto.SupplierMasterName),
        ["pvcMasterName"] = nameof(PVCInwardDto.PVCMasterName),
        ["new_RollNo"] = nameof(PVCInwardDto.New_RollNo),
        ["batchNo"] = nameof(PVCInwardDto.BatchNo),
        ["qty_kg"] = nameof(PVCInwardDto.Qty_kg),
        ["qty_Mtr"] = nameof(PVCInwardDto.Qty_Mtr),
        ["comments"] = nameof(PVCInwardDto.Comments),
        ["gramageName"] = nameof(PVCInwardDto.GramageName),
        ["widthName"] = nameof(PVCInwardDto.WidthName),
        ["colourName"] = nameof(PVCInwardDto.ColourName),
        ["billDate"] = nameof(PVCInwardDto.BillDate),
        ["receivedDate"] = nameof(PVCInwardDto.ReceivedDate),
        ["isActive"] = nameof(PVCInwardDto.IsActive),
    };

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

        var projectedQuery = q.Select(f => new PVCInwardDto
        {
            Id = f.Id,
            SupplierMasterId = f.SupplierMasterId,
            PVCMasterId = f.PVCMasterId,
            New_RollNo = f.New_RollNo,
            BatchNo = f.BatchNo,
            Qty_kg = f.Qty_kg,
            Qty_Mtr = f.Qty_Mtr,
            Comments = f.Comments,
            GramageMasterId = f.GramageMasterId,
            GramageName = f.GramageName,
            WidthMasterId = f.WidthMasterId,
            WidthName = f.WidthName,
            ColourMasterId = f.ColourMasterId,
            ColourName = f.ColourName,
            BillDate = f.BillDate,
            ReceivedDate = f.ReceivedDate,
            AttachedFile = f.AttachedFile,
            IsActive = f.IsActive,
            SupplierMasterName = f.Supplier != null ? f.Supplier.Name : string.Empty,
            PVCMasterName = f.PVC != null ? f.PVC.Name : string.Empty,
        });

        var normalizedSorts = NormalizeSorts(query.sort);
        projectedQuery = SortHelper.ApplySorting(projectedQuery, normalizedSorts, s => s.Field, s => s.Dir)
            ?? projectedQuery.OrderByDescending(n => n.Id);

        // Pagination
        var skip = (query.page - 1) * query.size;
        var items = await projectedQuery
            .Skip(skip)
            .Take(query.size)
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
            GramageMasterId = pvcInward.GramageMasterId,
            GramageName = pvcInward.GramageName,
            WidthMasterId = pvcInward.WidthMasterId,
            WidthName = pvcInward.WidthName,
            ColourMasterId = pvcInward.ColourMasterId,
            ColourName = pvcInward.ColourName,
            BillDate = pvcInward.BillDate,
            ReceivedDate = pvcInward.ReceivedDate,
            AttachedFile = pvcInward.AttachedFile,
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

            existing.SupplierMasterId = dto.SupplierMasterId;
            existing.PVCMasterId = dto.PVCMasterId;
            existing.New_RollNo = dto.New_RollNo;
            existing.BatchNo = dto.BatchNo;
            existing.Qty_kg = dto.Qty_kg;
            existing.Qty_Mtr = dto.Qty_Mtr;
            existing.Comments = dto.Comments;
            existing.GramageMasterId = dto.GramageMasterId;
            existing.GramageName = dto.GramageName;
            existing.WidthMasterId = dto.WidthMasterId;
            existing.WidthName = dto.WidthName;
            existing.ColourMasterId = dto.ColourMasterId;
            existing.ColourName = dto.ColourName;
            existing.BillDate = dto.BillDate;
            existing.ReceivedDate = dto.ReceivedDate;
            existing.AttachedFile = dto.AttachedFile;
            existing.IsActive = dto.IsActive ?? existing.IsActive;
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

    private static List<SortDto> NormalizeSorts(IList<SortDto>? sorts)
    {
        if (sorts == null || sorts.Count == 0)
        {
            return [];
        }

        return sorts
            .Where(s => !string.IsNullOrWhiteSpace(s.Field))
            .Select(s => new SortDto
            {
                Field = _sortFieldMap.TryGetValue(s.Field, out var mappedField) ? mappedField : s.Field,
                Dir = s.Dir,
            })
            .ToList();
    }
}
