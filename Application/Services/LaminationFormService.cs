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
        if (dto.FinalProductQtyMtr <= 0)
        {
            throw new ArgumentException("Final product quantity must be greater than zero");
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

        if (!dto.PVCInwardId.HasValue || dto.PVCInwardId <= 0)
            throw new ArgumentException("Select a PVC inward roll");

        var pvcInward = await _context.PVCInward.FirstOrDefaultAsync(x =>
            x.Id == dto.PVCInwardId.Value && (x.IsActive == null || x.IsActive == 1));
        if (pvcInward == null)
            throw new ArgumentException("PVC inward roll not found");
        if (dto.PVCQty <= 0)
            throw new ArgumentException("PVC quantity must be greater than zero");

        var pvcAlreadyUsed = await _context.LaminationForms
            .Where(x => x.PVCInwardId == pvcInward.Id)
            .SumAsync(x => x.PVCQty);
        if (pvcAlreadyUsed + dto.PVCQty > (decimal)pvcInward.Qty_kg)
            throw new ArgumentException($"Only {(decimal)pvcInward.Qty_kg - pvcAlreadyUsed:0.##} KG PVC is available in this inward batch");

        if (!dto.MixtureFormulaMasterId.HasValue || dto.MixtureFormulaMasterId <= 0)
            throw new ArgumentException("Select a chemical mixture");
        if (dto.MixtureQty <= 0)
            throw new ArgumentException("Mixture quantity must be greater than zero");

        var mixtureProduced = await _context.MixtureForms
            .Where(x => x.FormulaMasterId == dto.MixtureFormulaMasterId.Value)
            .SumAsync(x => x.TotalMixture);
        var mixtureUsed = await _context.LaminationForms
            .Where(x => x.MixtureFormulaMasterId == dto.MixtureFormulaMasterId.Value)
            .SumAsync(x => x.MixtureQty);
        if (mixtureProduced <= 0)
            throw new ArgumentException("Selected mixture has not been produced");
        if (mixtureUsed + dto.MixtureQty > mixtureProduced)
            throw new ArgumentException($"Only {mixtureProduced - mixtureUsed:0.##} KG mixture is available");

        var bondingUsed = dto.Bounding.Equals("Yes", StringComparison.OrdinalIgnoreCase);
        if (bondingUsed)
        {
            if (!dto.ChemicalId.HasValue || !await _context.Chemical.AnyAsync(x => x.Id == dto.ChemicalId.Value))
                throw new ArgumentException("Select a valid bonding chemical");
            if (dto.ChemicalQty <= 0)
                throw new ArgumentException("Bonding chemical quantity must be greater than zero");
        }
        else
        {
            dto.ChemicalId = null;
            dto.ChemicalQty = 0;
        }

        var workerExists = await _context.Employees.AnyAsync(x => x.Id == dto.WorkerId);
        if (!workerExists)
        {
            throw new ArgumentException("Worker not found");
        }

        var laminationForm = _mapper.Map<LaminationForm>(dto);
        laminationForm.PVCMasterId = pvcInward.PVCMasterId;
        laminationForm.PVCBatchNo = pvcInward.BatchNo;
        laminationForm.ClothRollBatchNo = string.IsNullOrWhiteSpace(clothRollingForm.BatchNo)
            ? dto.ClothRollBatchNo.Trim()
            : clothRollingForm.BatchNo.Trim();
        laminationForm.CreatedDate = dto.CreatedDate ?? DateTime.UtcNow;

        await _repository.AddAsync(laminationForm);
        await _context.SaveChangesAsync();

        var created = await _repository.GetByIdAsync(laminationForm.Id);
        return created == null ? MapDto(laminationForm) : MapDto(created);
    }

    public async Task<LaminationFormDto?> UpdateAsync(int id, LaminationFormDto dto)
    {
        var existing = await _context.LaminationForms.FirstOrDefaultAsync(x => x.Id == id);
        if (existing == null) return null;
        if (!await _context.FinalProduct.AnyAsync(x => x.Id == dto.FinalProductId))
            throw new ArgumentException("Final product not found");
        if (dto.FinalProductQtyMtr <= 0)
            throw new ArgumentException("Final product quantity must be greater than zero");
        if (!dto.ClothRollingFormId.HasValue)
            throw new ArgumentException("Select a cloth roll");
        var roll = await _context.ClothRollingForms.FirstOrDefaultAsync(x => x.Id == dto.ClothRollingFormId.Value)
            ?? throw new ArgumentException("Cloth roll not found");
        if (!dto.PVCInwardId.HasValue || dto.PVCQty <= 0)
            throw new ArgumentException("Select PVC inward and enter quantity");
        var pvcInward = await _context.PVCInward.FirstOrDefaultAsync(x => x.Id == dto.PVCInwardId.Value)
            ?? throw new ArgumentException("PVC inward not found");
        var pvcUsed = await _context.LaminationForms.Where(x => x.Id != id && x.PVCInwardId == pvcInward.Id).SumAsync(x => x.PVCQty);
        if (pvcUsed + dto.PVCQty > (decimal)pvcInward.Qty_kg)
            throw new ArgumentException($"Only {(decimal)pvcInward.Qty_kg - pvcUsed:0.##} KG PVC is available");
        if (!dto.MixtureFormulaMasterId.HasValue || dto.MixtureQty <= 0)
            throw new ArgumentException("Select mixture and enter quantity");
        var produced = await _context.MixtureForms.Where(x => x.FormulaMasterId == dto.MixtureFormulaMasterId.Value).SumAsync(x => x.TotalMixture);
        var mixtureUsed = await _context.LaminationForms.Where(x => x.Id != id && x.MixtureFormulaMasterId == dto.MixtureFormulaMasterId.Value).SumAsync(x => x.MixtureQty);
        if (mixtureUsed + dto.MixtureQty > produced)
            throw new ArgumentException($"Only {produced - mixtureUsed:0.##} KG mixture is available");
        if (!await _context.Employees.AnyAsync(x => x.Id == dto.WorkerId))
            throw new ArgumentException("Worker not found");

        var bondingUsed = dto.Bounding.Equals("Yes", StringComparison.OrdinalIgnoreCase);
        if (bondingUsed && (!dto.ChemicalId.HasValue || dto.ChemicalQty <= 0 || !await _context.Chemical.AnyAsync(x => x.Id == dto.ChemicalId.Value)))
            throw new ArgumentException("Select bonding chemical and enter quantity");

        existing.FinalProductId = dto.FinalProductId;
        existing.FinalProductQtyMtr = dto.FinalProductQtyMtr;
        existing.ClothRollingFormId = dto.ClothRollingFormId;
        existing.ClothRollBatchNo = roll.BatchNo;
        existing.PVCInwardId = pvcInward.Id;
        existing.PVCMasterId = pvcInward.PVCMasterId;
        existing.PVCBatchNo = pvcInward.BatchNo;
        existing.PVCQty = dto.PVCQty;
        existing.MixtureFormulaMasterId = dto.MixtureFormulaMasterId;
        existing.MixtureQty = dto.MixtureQty;
        existing.Bounding = bondingUsed ? "Yes" : "No";
        existing.ChemicalId = bondingUsed ? dto.ChemicalId : null;
        existing.ChemicalQty = bondingUsed ? dto.ChemicalQty : 0;
        existing.WorkerId = dto.WorkerId;
        existing.Temperature = dto.Temperature;
        existing.ProcessTime = dto.ProcessTime.Trim();
        await _context.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    private static LaminationFormDto MapDto(LaminationForm laminationForm) =>
        new()
        {
            Id = laminationForm.Id,
            FinalProductId = laminationForm.FinalProductId,
            ClothRollingFormId = laminationForm.ClothRollingFormId,
            ClothRollCode = laminationForm.ClothRollingForm?.RollNo ?? laminationForm.ClothRollingFormId?.ToString() ?? string.Empty,
            ClothRollBatchNo = string.IsNullOrWhiteSpace(laminationForm.ClothRollBatchNo)
                ? laminationForm.ClothRollingForm?.BatchNo ?? string.Empty
                : laminationForm.ClothRollBatchNo,
            PVCMasterId = laminationForm.PVCMasterId,
            PVCInwardId = laminationForm.PVCInwardId,
            PVCBatchNo = laminationForm.PVCBatchNo,
            PVCQty = laminationForm.PVCQty,
            MixtureFormulaMasterId = laminationForm.MixtureFormulaMasterId,
            MixtureQty = laminationForm.MixtureQty,
            FinalProductQtyMtr = laminationForm.FinalProductQtyMtr,
            ChemicalId = laminationForm.ChemicalId,
            ChemicalQty = laminationForm.ChemicalQty,
            Bounding = laminationForm.Bounding,
            WorkerId = laminationForm.WorkerId,
            Temperature = laminationForm.Temperature,
            ProcessTime = laminationForm.ProcessTime,
            CreatedDate = laminationForm.CreatedDate,
            FinalProductName = laminationForm.FinalProduct?.Final_Product ?? string.Empty,
            PVCName = laminationForm.PVC?.Name ?? string.Empty,
            MixtureName = laminationForm.MixtureFormulaMaster == null
                ? string.Empty
                : $"{laminationForm.MixtureFormulaMaster.FinalProduct?.Final_Product} - {laminationForm.MixtureFormulaMaster.MixtureName}".Trim(' ', '-'),
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
