using System.Diagnostics;
using Api.Application.DTOs;
using Api.Application.Interfaces;
using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Api.Infrastructure.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Services;

public class FormulaChemicalTransactionService(
    IFormulaChemicalTransactionRepository _repository,
    IMapper _mapper,
    AppDbContext _context
) : IFormulaChemicalTransactionService
{
    private static readonly string[] _excludedSearchProperties = ["IsActive", "Id", "SrNo"];

    public async Task<PagedResultDto<FormulaChemicalTransactionDto>> GetAllAsync(
    PagedQueryDto query
)
    {
        // Project to DTO first (NO Include needed)
        var q = _repository
            .Query()
            .Select(x => new FormulaChemicalTransactionDto
            {
                Id = x.Id,
                FormulaMasterId = x.FormulaMasterId,
                ChemicalMasterId = x.ChemicalMasterId,
                Qty = x.Qty,
                FinalProductName = x.FormulaMaster.FinalProduct.Final_Product,
                ChemicalMasterName = x.Chemical.Name
            });

        // ✅ Apply global search on DTO
        if (query.filter.Any(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)))
        {
            var searchTerms = query.filter
                .Where(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase))
                .Select(f => f.Value)
                .ToList();

            q = q.Where(
                SearchHelper.BuildGlobalSearchPredicate<FormulaChemicalTransactionDto>(
                    searchTerms,
                    _excludedSearchProperties
                )
            );
        }

        var total = await q.CountAsync();

        // ✅ Apply sorting on DTO fields
        q =
            SortHelper.ApplySorting(q, query.sort, s => s.Field, s => s.Dir)
            ?? q.OrderByDescending(n => n.Id);

        // ✅ Pagination
        var skip = (query.page - 1) * query.size;

        var items = await q
            .Skip(skip)
            .Take(query.size)
            .ToListAsync();

        return new PagedResultDto<FormulaChemicalTransactionDto>
        {
            Items = items,
            TotalCount = total,
            Page = query.page,
            Size = query.size,
        };
    }

    public async Task<FormulaTransactionDetailsDto?> GetByIdAsync(int formulaMasterId)
    {
        var data = await _context.FormulaChemicalTransaction
            .Include(x => x.Chemical)
            .Include(x => x.FormulaMaster)
                .ThenInclude(f => f.FinalProduct)
            .Where(x => x.FormulaMasterId == formulaMasterId)
            .ToListAsync();

        if (!data.Any())
            return null;

        var first = data.First();

        return new FormulaTransactionDetailsDto
        {
            FormulaMasterId = first.FormulaMasterId,
            FinalProductId = first.FormulaMaster.FinalProductId,

            Chemicals = data.Select(x => new ChemicalItemDto
            {
                ChemicalMasterId = x.ChemicalMasterId,
                Qty = x.Qty,
                ChemicalName = x.Chemical.Name
            }).ToList()
        };
    }


    public async Task<FormulaChemicalTransactionDto> CreateAsync(FormulaChemicalTransactionDto dto)
    {
        // Prevent duplicates
        // bool exists = await _context.FormulaChemicalTransaction
        //     .AnyAsync(e => e.FormulaMasterId == dto.FormulaMasterId
        //                && e.ChemicalMasterId == dto.ChemicalMasterId);

        // if (exists)
        //     throw new ArgumentException("This chemical already exists for the formula");

        // Map DTO → entity, ignoring Id and navigation properties
        var entity = _mapper.Map<FormulaChemicalTransaction>(dto);

        await _repository.AddAsync(entity);

        await _context.SaveChangesAsync();  // Id is generated here

        return _mapper.Map<FormulaChemicalTransactionDto>(entity);  // DTO now includes generated Id
    }


    public async Task<bool> DeleteAsync(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var FormulaChemicalTransaction = await _repository.GetByIdAsync(id);
            if (FormulaChemicalTransaction == null)
                return false;

            // Delete Gramage
            _context.FormulaChemicalTransaction.Remove(FormulaChemicalTransaction);
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


    //newly added for one to many 
    // 1️⃣ Implement GetFormulaDetailsAsync
    public async Task<FormulaDetailsDto?> GetFormulaDetailsAsync(int formulaMasterId)
    {
        var chemicals = await _repository.Query()
            .Where(c => c.FormulaMasterId == formulaMasterId)
            .Include(c => c.Chemical)
            .ToListAsync();

        if (!chemicals.Any()) return null;

        return new FormulaDetailsDto
        {
            FormulaMasterId = formulaMasterId,
            FinalProductId = chemicals.First().FormulaMasterId,
            Chemicals = chemicals.Select(c => new ChemicalItemDto
            {
                ChemicalMasterId = c.ChemicalMasterId,
                Qty = c.Qty,
                ChemicalName = c.Chemical.Name
            }).ToList()
        };
    }

    // 2️⃣ Implement UpdateFormulaAsync
    public async Task<FormulaDetailsDto?> UpdateFormulaAsync(FormulaDetailsDto dto)
    {
        var existingChemicals = await _repository.Query()
            .Where(c => c.FormulaMasterId == dto.FormulaMasterId)
            .ToListAsync();

        if (!existingChemicals.Any()) return null;

        foreach (var chemDto in dto.Chemicals)
        {
            var entity = existingChemicals
                .FirstOrDefault(c => c.ChemicalMasterId == chemDto.ChemicalMasterId);
            if (entity != null)
            {
                entity.Qty = chemDto.Qty;
                await _repository.UpdateAsync(entity.Id, entity);
            }
        }

        await _context.SaveChangesAsync();

        return new FormulaDetailsDto
        {
            FormulaMasterId = dto.FormulaMasterId,
            FinalProductId = existingChemicals.First().FormulaMasterId,
            Chemicals = existingChemicals.Select(c => new ChemicalItemDto
            {
                ChemicalMasterId = c.ChemicalMasterId,
                Qty = c.Qty,
                ChemicalName = c.Chemical.Name
            }).ToList()
        };
    }


    public async Task<FormulaChemicalTransactionDto?> UpdateAsync(
    int id,
    FormulaChemicalTransactionDto dto
)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            return null;

        existing.ChemicalMasterId = dto.ChemicalMasterId;
        existing.FormulaMasterId = dto.FormulaMasterId;
        existing.Qty = dto.Qty;

        var updated = await _repository.UpdateAsync(id, existing);

        await _context.SaveChangesAsync();

        return updated == null
            ? null
            : _mapper.Map<FormulaChemicalTransactionDto>(updated);
    }

}


// using Api.Application.DTOs;
// using Api.Application.Interfaces;
// using Api.Domain.Entities;
// using Api.Infrastructure.Data;
// using Api.Infrastructure.Repositories;
// using AutoMapper;
// using Microsoft.EntityFrameworkCore;

// namespace Api.Application.Services;

// public class FormulaChemicalTransactionService : IFormulaChemicalTransactionService
// {
//     private readonly IFormulaChemicalTransactionRepository _repository;
//     private readonly IMapper _mapper;
//     private readonly AppDbContext _context;

//     public FormulaChemicalTransactionService(
//         IFormulaChemicalTransactionRepository repository,
//         IMapper mapper,
//         AppDbContext context
//     )
//     {
//         _repository = repository;
//         _mapper = mapper;
//         _context = context;
//     }

//     // ------------------------------
//     // GET all formulas with their chemicals (one-to-many) with optional pagination
//     public async Task<PagedResultDto<FormulaDetailsDto>> GetAllFormulasAsync(PagedQueryDto query)
//     {
//         // Query all transactions with related entities
//         var q = _repository.Query()
//             .Include(x => x.Chemical)
//             .Include(x => x.FormulaMaster)
//                 .ThenInclude(f => f.FinalProduct);

//         // Group by FormulaMasterId to create one-to-many structure
//         var grouped = q
//             .AsEnumerable()
//             .GroupBy(x => x.FormulaMasterId)
//             .Select(g => new FormulaDetailsDto
//             {
//                 FormulaMasterId = g.Key,
//                 FinalProductId = g.First().FormulaMaster.FinalProductId,
//                 Chemicals = g.Select(c => new ChemicalItemDto
//                 {
//                     ChemicalMasterId = c.ChemicalMasterId,
//                     Qty = c.Qty,
//                     ChemicalName = c.Chemical.Name
//                 }).ToList()
//             });

//         // Pagination
//         var total = grouped.Count();
//         var items = grouped
//             .Skip((query.page - 1) * query.size)
//             .Take(query.size)
//             .ToList();

//         return new PagedResultDto<FormulaDetailsDto>
//         {
//             Items = items,
//             TotalCount = total,
//             Page = query.page,
//             Size = query.size
//         };
//     }

//     // ------------------------------
//     // GET single formula with all chemicals (one-to-many)
//     public async Task<FormulaDetailsDto?> GetFormulaDetailsAsync(int formulaMasterId)
//     {
//         var chemicals = await _repository.Query()
//             .Where(c => c.FormulaMasterId == formulaMasterId)
//             .Include(c => c.Chemical)
//             .Include(c => c.FormulaMaster)
//                 .ThenInclude(f => f.FinalProduct)
//             .ToListAsync();

//         if (!chemicals.Any()) return null;

//         return new FormulaDetailsDto
//         {
//             FormulaMasterId = formulaMasterId,
//             FinalProductId = chemicals.First().FormulaMaster.FinalProductId,
//             Chemicals = chemicals.Select(c => new ChemicalItemDto
//             {
//                 ChemicalMasterId = c.ChemicalMasterId,
//                 Qty = c.Qty,
//                 ChemicalName = c.Chemical.Name
//             }).ToList()
//         };
//     }

//     // ------------------------------
//     // CREATE a new chemical entry
//     public async Task<FormulaChemicalTransactionDto> CreateAsync(FormulaChemicalTransactionDto dto)
//     {
//         var entity = _mapper.Map<FormulaChemicalTransaction>(dto);
//         await _repository.AddAsync(entity);
//         await _context.SaveChangesAsync();
//         return _mapper.Map<FormulaChemicalTransactionDto>(entity);
//     }

//     // ------------------------------
//     // DELETE a chemical
//     public async Task<bool> DeleteAsync(int id)
//     {
//         using var transaction = await _context.Database.BeginTransactionAsync();
//         try
//         {
//             var entity = await _repository.GetByIdAsync(id);
//             if (entity == null) return false;

//             _context.FormulaChemicalTransaction.Remove(entity);
//             await _context.SaveChangesAsync();

//             await transaction.CommitAsync();
//             return true;
//         }
//         catch
//         {
//             await transaction.RollbackAsync();
//             throw;
//         }
//     }

//     // ------------------------------
//     // UPDATE a formula with multiple chemicals (one-to-many)
//     public async Task<FormulaDetailsDto?> UpdateFormulaAsync(FormulaDetailsDto dto)
//     {
//         var existingChemicals = await _repository.Query()
//             .Where(c => c.FormulaMasterId == dto.FormulaMasterId)
//             .ToListAsync();

//         if (!existingChemicals.Any()) return null;

//         foreach (var chemDto in dto.Chemicals)
//         {
//             var entity = existingChemicals.FirstOrDefault(c => c.ChemicalMasterId == chemDto.ChemicalMasterId);
//             if (entity != null)
//             {
//                 entity.Qty = chemDto.Qty;
//                 await _repository.UpdateAsync(entity.Id, entity);
//             }
//         }

//         await _context.SaveChangesAsync();

//         return new FormulaDetailsDto
//         {
//             FormulaMasterId = dto.FormulaMasterId,
//             FinalProductId = existingChemicals.First().FormulaMasterId,
//             Chemicals = existingChemicals.Select(c => new ChemicalItemDto
//             {
//                 ChemicalMasterId = c.ChemicalMasterId,
//                 Qty = c.Qty,
//                 ChemicalName = c.Chemical.Name
//             }).ToList()
//         };
//     }
// }
