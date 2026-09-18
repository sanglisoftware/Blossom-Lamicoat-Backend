using Api.Application.DTOs;
using Api.Application.Interfaces;
using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Api.Infrastructure.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Services;

public class ClothRollingFormService(
    IClothRollingFormRepository _repository,
    IMapper _mapper,
    AppDbContext _context
) : IClothRollingFormService
{
    private static readonly string[] _excludedSearchProperties = ["Id"];

    public async Task<PagedResultDto<ClothRollingFormDto>> GetAllAsync(PagedQueryDto query)
    {
        var q = _repository.Query();

        if (query.filter.Any(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)))
        {
            var searchTerms = query.filter
                .Where(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase))
                .Select(f => f.Value)
                .ToList();

            q = q.Where(
                SearchHelper.BuildGlobalSearchPredicate<ClothRollingForm>(
                    searchTerms,
                    _excludedSearchProperties
                )
            );
        }

        var total = await q.CountAsync();

        q =
            SortHelper.ApplySorting(q, query.sort, s => s.Field, s => s.Dir)
            ?? q.OrderByDescending(x => x.Id);

        var skip = (query.page - 1) * query.size;
        var items = await q.Skip(skip).Take(query.size).ToListAsync();

        return new PagedResultDto<ClothRollingFormDto>
        {
            Items = items.Select(MapDto),
            TotalCount = total,
            Page = query.page,
            Size = query.size,
        };
    }

    public async Task<ClothRollingFormDto?> GetByIdAsync(int id)
    {
        var clothRollingForm = await _repository.GetByIdAsync(id);
        return clothRollingForm == null ? null : MapDto(clothRollingForm);
    }

    public async Task<ClothRollingFormDto> CreateAsync(ClothRollingFormDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.ProductName))
        {
            throw new ArgumentException("Product name is required");
        }

        if (string.IsNullOrWhiteSpace(dto.BatchNo))
        {
            throw new ArgumentException("Batch no is required");
        }

        if (string.IsNullOrWhiteSpace(dto.CheckerName))
        {
            throw new ArgumentException("Checker name is required");
        }

        if (dto.RollMtr <= 0)
        {
            throw new ArgumentException("Roll MTR must be greater than zero");
        }

        if (dto.DefectMtr < 0)
        {
            throw new ArgumentException("Defect MTR cannot be negative");
        }

        if (dto.DefectMtr > dto.RollMtr)
        {
            throw new ArgumentException("Defect MTR cannot be greater than actual roll MTR");
        }

        if (!dto.FabricInwardId.HasValue || dto.FabricInwardId <= 0)
        {
            throw new ArgumentException("Select a fabric inward batch");
        }

        var productName = dto.ProductName.Trim();
        var batchNo = dto.BatchNo.Trim();
        var checkerName = dto.CheckerName.Trim();

        var fabricInward = await _context.FabricInward
            .Include(x => x.Fabric)
            .FirstOrDefaultAsync(x =>
                x.Id == dto.FabricInwardId.Value
                && (x.IsActive == null || x.IsActive == 1)
                && x.Fabric != null
                && x.Fabric.Name == productName
                && x.BatchNo == batchNo
            );

        if (fabricInward == null)
        {
            throw new ArgumentException("Selected product and batch no were not found in fabric inward");
        }

        var alreadyProcessedMtr = await _context.ClothRollingForms
            .Where(x => x.FabricInwardId == fabricInward.Id && (x.IsActive == null || x.IsActive == 1))
            .SumAsync(x => x.RollMtr + x.DefectMtr);

        if (alreadyProcessedMtr >= (decimal)fabricInward.QtyMTR)
        {
            throw new ArgumentException("All inward MTR for this batch has already been rolled");
        }

        var nextRollSequence = (await _context.ClothRollingForms.MaxAsync(x => (int?)x.Id) ?? 0) + 1;
        var clothRollingForm = _mapper.Map<ClothRollingForm>(dto);
        clothRollingForm.ProductName = productName;
        clothRollingForm.BatchNo = batchNo;
        clothRollingForm.FabricInwardId = fabricInward.Id;
        clothRollingForm.RollNo = $"R-{DateTime.Now:ddMMyyyy}-{nextRollSequence:D5}";
        clothRollingForm.CheckerName = checkerName;
        clothRollingForm.CreatedDate = dto.CreatedDate ?? DateTime.UtcNow;
        clothRollingForm.IsActive = dto.IsActive ?? 1;

        await _repository.AddAsync(clothRollingForm);
        await _context.SaveChangesAsync();

        return MapDto(clothRollingForm);
    }

    private static ClothRollingFormDto MapDto(ClothRollingForm clothRollingForm) =>
        new()
        {
            Id = clothRollingForm.Id,
            FabricInwardId = clothRollingForm.FabricInwardId,
            RollNo = clothRollingForm.RollNo,
            ProductName = clothRollingForm.ProductName,
            Gramage = clothRollingForm.FabricInward?.FGramage?.GRM ?? string.Empty,
            Colour = clothRollingForm.FabricInward?.Colour?.Name ?? string.Empty,
            BatchNo = clothRollingForm.BatchNo,
            RollMtr = clothRollingForm.RollMtr,
            DefectMtr = clothRollingForm.DefectMtr,
            CheckerName = clothRollingForm.CheckerName,
            IsActive = clothRollingForm.IsActive,
            CreatedDate = clothRollingForm.CreatedDate,
        };
}
