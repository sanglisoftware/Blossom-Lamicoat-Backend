using Api.Application.DTOs;
using Api.Application.Interfaces;
using AutoMapper;
using System.Linq.Expressions;
using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Api.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Services;

public class ChemicalInwardService(IChemicalInwardRepository _repository, IMapper _mapper, AppDbContext _context) : IChemicalInwardService
{
    private static readonly string[] _excludedSearchProperties = ["IsActive", "Id", "SrNo",];

    public async Task<PagedResultDto<ChemicalInwardDto>> GetAllAsync(PagedQueryDto query)
    {
        var q = _repository.Query();

        // Apply global search (including related Chemical and Supplier names)
        if (query.filter.Any(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)))
        {
            var searchTerms = query.filter.Where(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)).Select(f => f.Value).ToList();

            // base predicate for ChemicalInward properties
            var basePredicate = SearchHelper.BuildGlobalSearchPredicate<ChemicalInward>(searchTerms, _excludedSearchProperties);

            // build predicate for related entity name fields
            Expression<Func<ChemicalInward, bool>> relatedPredicate = ci => false;
            foreach (var term in searchTerms.Where(t => !string.IsNullOrWhiteSpace(t)))
            {
                Expression<Func<ChemicalInward, bool>> termPred = ci =>
                    (ci.Chemical != null && EF.Functions.Like(ci.Chemical.Name, "%" + term + "%")) ||
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
        var items = await q.Skip(skip).Take(query.size).ToListAsync();

        return new PagedResultDto<ChemicalInwardDto>
        {
            Items = items.Select(_mapper.Map<ChemicalInwardDto>),
            TotalCount = total,
            Page = query.page,
            Size = query.size,
        };
    }

    public async Task<ChemicalInwardDto?> GetByIdAsync(int id)
    {
        var chemicalInward = await _context.ChemicalInward.FirstOrDefaultAsync(e => e.Id == id);
        if (chemicalInward == null) return null;

        return new ChemicalInwardDto
        {
            Id = chemicalInward.Id,
            ChemicalMasterId = chemicalInward.ChemicalMasterId,
            Qty = chemicalInward.Qty,
            SupplierMasterId = chemicalInward.SupplierMasterId,
            BatchNo = chemicalInward.BatchNo,
            BillDate = chemicalInward.BillDate,
            ReceivedDate = chemicalInward.ReceivedDate,
            // ChemicalMasterName = chemicalInward.Name,
            // SupplierMasterName = chemicalInward.SupplierMaster.Name,
        };
    }

    public async Task<ChemicalInwardDto> CreateAsync(ChemicalInwardDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Check GRM exists in either table
            if (await _context.ChemicalInward.AnyAsync(e => e.BatchNo == dto.BatchNo))
            {
                throw new ArgumentException("Batch No already exists");
            }

            // 2. Create Final product
            var chemicalInward = _mapper.Map<ChemicalInward>(dto);
            await _repository.AddAsync(chemicalInward);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return _mapper.Map<ChemicalInwardDto>(chemicalInward);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<ChemicalInwardDto?> UpdateAsync(int id, ChemicalInwardDto dto)
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
            return _mapper.Map<ChemicalInwardDto>(existing);
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
            var chemicalInward = await _repository.GetByIdAsync(id);
            if (chemicalInward == null) return false;

            // Delete ChemicalInward
            _context.ChemicalInward.Remove(chemicalInward);
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

    public async Task<ChemicalInwardDto?> UpdateStatusAsync(int id, short IsActive)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;
        existing.IsActive = IsActive;
        var updated = await _repository.UpdateAsync(id, existing);
        return updated is null ? null : _mapper.Map<ChemicalInwardDto>(updated);
    }
}
