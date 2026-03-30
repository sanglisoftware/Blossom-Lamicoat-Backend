using Api.Application.DTOs;
using Api.Application.Interfaces;
using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Api.Infrastructure.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Services;

public class InspectionFormService(
    IInspectionFormRepository _repository,
    IMapper _mapper,
    AppDbContext _context
) : IInspectionFormService
{
    private static readonly string[] _excludedSearchProperties = ["Id", "ManufacturedFabricProductId", "GradeId"];

    public async Task<PagedResultDto<InspectionFormDto>> GetAllAsync(PagedQueryDto query)
    {
        var q = _repository.Query();

        if (query.filter.Any(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)))
        {
            var searchTerms = query.filter
                .Where(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase))
                .Select(f => f.Value)
                .ToList();

            q = q.Where(
                SearchHelper.BuildGlobalSearchPredicate<InspectionForm>(
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

        return new PagedResultDto<InspectionFormDto>
        {
            Items = items.Select(MapDto),
            TotalCount = total,
            Page = query.page,
            Size = query.size,
        };
    }

    public async Task<InspectionFormDto?> GetByIdAsync(int id)
    {
        var inspectionForm = await _repository.GetByIdAsync(id);
        return inspectionForm == null ? null : MapDto(inspectionForm);
    }

    public async Task<InspectionFormDto> CreateAsync(InspectionFormDto dto)
    {
        var fabricProduct = await _context.FproductList.FirstOrDefaultAsync(x => x.Id == dto.ManufacturedFabricProductId);
        if (fabricProduct == null)
        {
            throw new ArgumentException("Manufactured fabric product not found");
        }

        var grade = await _context.Grade.FirstOrDefaultAsync(x => x.Id == dto.GradeId);
        if (grade == null)
        {
            throw new ArgumentException("Grade not found");
        }

        var inspectionForm = _mapper.Map<InspectionForm>(dto);
        inspectionForm.CreatedDate = dto.CreatedDate ?? DateTime.UtcNow;

        await _repository.AddAsync(inspectionForm);
        await _context.SaveChangesAsync();

        var created = await _repository.GetByIdAsync(inspectionForm.Id);
        return created == null ? MapDto(inspectionForm) : MapDto(created);
    }

    private static InspectionFormDto MapDto(InspectionForm inspectionForm) =>
        new()
        {
            Id = inspectionForm.Id,
            ManufacturedFabricProductId = inspectionForm.ManufacturedFabricProductId,
            GradeId = inspectionForm.GradeId,
            Mtr = inspectionForm.Mtr,
            WastageMtr = inspectionForm.WastageMtr,
            CreatedDate = inspectionForm.CreatedDate,
            ManufacturedFabricProductName = inspectionForm.ManufacturedFabricProduct?.Name ?? string.Empty,
            GradeName = inspectionForm.Grade?.Name ?? string.Empty,
        };
}
