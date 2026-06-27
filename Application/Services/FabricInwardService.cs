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
        var q = _context.FabricInward.AsQueryable();

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
                    (_context.FproductList.Any(fp => fp.Id == ci.FabricMasterId && EF.Functions.Like(fp.Name, "%" + term + "%"))) ||
                    (_context.Supplier.Any(s => s.Id == ci.SupplierMasterId && EF.Functions.Like(s.Name, "%" + term + "%"))) ||
                    (ci.FGramageMasterId != null && _context.FGramage.Any(g => g.Id == ci.FGramageMasterId && EF.Functions.Like(g.GRM, "%" + term + "%"))) ||
                    (ci.ColourMasterId != null && _context.Colour.Any(c => c.Id == ci.ColourMasterId && EF.Functions.Like(c.Name, "%" + term + "%")));

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
        FGramageMasterId = f.FGramageMasterId,
        ColourMasterId = f.ColourMasterId,
        BatchNo = f.BatchNo,
        QtyMTR = f.QtyMTR,
        Comments = f.Comments,
        AttachedFile = f.AttachedFile,
        IsActive = f.IsActive,
        SupplierMasterName = _context.Supplier
            .Where(s => s.Id == f.SupplierMasterId)
            .Select(s => s.Name)
            .FirstOrDefault() ?? string.Empty,
        FabricMasterName = _context.FproductList
            .Where(fp => fp.Id == f.FabricMasterId)
            .Select(fp => fp.Name)
            .FirstOrDefault() ?? string.Empty,
        FGramageMasterName = f.FGramageMasterId != null
            ? (_context.FGramage
                .Where(g => g.Id == f.FGramageMasterId)
                .Select(g => g.GRM)
                .FirstOrDefault() ?? string.Empty)
            : string.Empty,
        ColourMasterName = f.ColourMasterId != null
            ? (_context.Colour
                .Where(c => c.Id == f.ColourMasterId)
                .Select(c => c.Name)
                .FirstOrDefault() ?? string.Empty)
            : string.Empty,
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
            .FirstOrDefaultAsync(e => e.Id == id);
        if (fabricInward == null) return null;

        return new FabricInwardDto
        {
            Id = fabricInward.Id,
            SupplierMasterId = fabricInward.SupplierMasterId,
            FabricMasterId = fabricInward.FabricMasterId,
            FGramageMasterId = fabricInward.FGramageMasterId,
            ColourMasterId = fabricInward.ColourMasterId,
            BatchNo = fabricInward.BatchNo,
            QtyMTR = fabricInward.QtyMTR,
            Comments = fabricInward.Comments,
            AttachedFile = fabricInward.AttachedFile,
            IsActive = fabricInward.IsActive,
            SupplierMasterName = await _context.Supplier
                .Where(s => s.Id == fabricInward.SupplierMasterId)
                .Select(s => s.Name)
                .FirstOrDefaultAsync(),
            FabricMasterName = await _context.FproductList
                .Where(fp => fp.Id == fabricInward.FabricMasterId)
                .Select(fp => fp.Name)
                .FirstOrDefaultAsync(),
            FGramageMasterName = fabricInward.FGramageMasterId != null
                ? await _context.FGramage
                    .Where(g => g.Id == fabricInward.FGramageMasterId)
                    .Select(g => g.GRM)
                    .FirstOrDefaultAsync()
                : null,
            ColourMasterName = fabricInward.ColourMasterId != null
                ? await _context.Colour
                    .Where(c => c.Id == fabricInward.ColourMasterId)
                    .Select(c => c.Name)
                    .FirstOrDefaultAsync()
                : null,
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

            var fabricInward = new FabricInward
            {
                SupplierMasterId = dto.SupplierMasterId,
                FabricMasterId = dto.FabricMasterId,
                FGramageMasterId = dto.FGramageMasterId,
                ColourMasterId = dto.ColourMasterId,
                BatchNo = dto.BatchNo,
                QtyMTR = dto.QtyMTR,
                Comments = dto.Comments,
                AttachedFile = dto.AttachedFile,
                IsActive = dto.IsActive,
            };

            await _repository.AddAsync(fabricInward);
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
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            existing.SupplierMasterId = dto.SupplierMasterId;
            existing.FabricMasterId = dto.FabricMasterId;
            existing.FGramageMasterId = dto.FGramageMasterId;
            existing.ColourMasterId = dto.ColourMasterId;
            existing.BatchNo = dto.BatchNo;
            existing.QtyMTR = dto.QtyMTR;
            existing.Comments = dto.Comments;
            existing.AttachedFile = dto.AttachedFile;
            existing.IsActive = dto.IsActive ?? existing.IsActive;

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
