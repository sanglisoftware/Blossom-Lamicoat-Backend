using Api.Application.DTOs;
using Api.Application.Interfaces;
using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Api.Infrastructure.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Services;

public class LaminationFormService(
    ILaminationFormRepository _repository,
    IMapper _mapper,
    AppDbContext _context
) : ILaminationFormService
{
    private static readonly string[] _excludedSearchProperties =
        ["Id", "FinalProductId", "PVCMasterId", "ChemicalId", "WorkerId"];

    public async Task<PagedResultDto<LaminationFormDto>> GetAllAsync(PagedQueryDto query)
    {
        var q = _repository.Query();

        if (query.filter.Any(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)))
        {
            var searchTerms = query.filter
                .Where(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase))
                .Select(f => f.Value)
                .ToList();

            q = q.Where(
                SearchHelper.BuildGlobalSearchPredicate<LaminationForm>(
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

        return new PagedResultDto<LaminationFormDto>
        {
            Items = items.Select(MapDto),
            TotalCount = total,
            Page = query.page,
            Size = query.size,
        };
    }

    public async Task<LaminationFormDto?> GetByIdAsync(int id)
    {
        var laminationForm = await _repository.GetByIdAsync(id);
        return laminationForm == null ? null : MapDto(laminationForm);
    }

    public async Task<LaminationFormDto> CreateAsync(LaminationFormDto dto)
    {
        var finalProduct = await _context.FinalProduct.FirstOrDefaultAsync(x => x.Id == dto.FinalProductId);
        if (finalProduct == null)
        {
            throw new ArgumentException("Final product not found");
        }

        if (!dto.ClothRollingFormId.HasValue)
        {
            throw new ArgumentException("Cloth rolling form is required");
        }

        var clothRollingForm = await _context.ClothRollingForms
            .FirstOrDefaultAsync(x => x.Id == dto.ClothRollingFormId.Value);
        if (clothRollingForm == null)
        {
            throw new ArgumentException("Cloth rolling form not found");
        }

        if (dto.PVCMasterId.HasValue)
        {
            var pvcExists = await _context.PVCproductList.AnyAsync(x => x.Id == dto.PVCMasterId.Value);
            if (!pvcExists)
            {
                throw new ArgumentException("PVC not found");
            }
        }

        var chemicalExists = await _context.Chemical.AnyAsync(x => x.Id == dto.ChemicalId);
        if (!chemicalExists)
        {
            throw new ArgumentException("Chemical not found");
        }

        var workerExists = await _context.Employees.AnyAsync(x => x.Id == dto.WorkerId);
        if (!workerExists)
        {
            throw new ArgumentException("Worker not found");
        }

        var laminationForm = _mapper.Map<LaminationForm>(dto);
        laminationForm.ClothRollBatchNo = string.IsNullOrWhiteSpace(clothRollingForm.BatchNo)
            ? dto.ClothRollBatchNo.Trim()
            : clothRollingForm.BatchNo.Trim();
        laminationForm.CreatedDate = dto.CreatedDate ?? DateTime.UtcNow;

        await _repository.AddAsync(laminationForm);
        await _context.SaveChangesAsync();

        var created = await _repository.GetByIdAsync(laminationForm.Id);
        return created == null ? MapDto(laminationForm) : MapDto(created);
    }

    private static LaminationFormDto MapDto(LaminationForm laminationForm) =>
        new()
        {
            Id = laminationForm.Id,
            FinalProductId = laminationForm.FinalProductId,
            ClothRollingFormId = laminationForm.ClothRollingFormId,
            ClothRollCode = laminationForm.ClothRollingFormId?.ToString() ?? string.Empty,
            ClothRollBatchNo = string.IsNullOrWhiteSpace(laminationForm.ClothRollBatchNo)
                ? laminationForm.ClothRollingForm?.BatchNo ?? string.Empty
                : laminationForm.ClothRollBatchNo,
            PVCMasterId = laminationForm.PVCMasterId,
            PVCBatchNo = laminationForm.PVCBatchNo,
            PVCQty = laminationForm.PVCQty,
            ChemicalId = laminationForm.ChemicalId,
            ChemicalQty = laminationForm.ChemicalQty,
            Bounding = laminationForm.Bounding,
            WorkerId = laminationForm.WorkerId,
            Temperature = laminationForm.Temperature,
            ProcessTime = laminationForm.ProcessTime,
            CreatedDate = laminationForm.CreatedDate,
            FinalProductName = laminationForm.FinalProduct?.Final_Product ?? string.Empty,
            PVCName = laminationForm.PVC?.Name ?? string.Empty,
            ChemicalName = laminationForm.Chemical?.Name ?? string.Empty,
            WorkerName = string.Join(
                " ",
                new[]
                {
                    laminationForm.Worker?.FirstName,
                    laminationForm.Worker?.MiddleName,
                    laminationForm.Worker?.LastName,
                }.Where(x => !string.IsNullOrWhiteSpace(x))
            ),
        };
}
