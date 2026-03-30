using Api.Application.DTOs;
using Api.Application.Interfaces;
using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Api.Infrastructure.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Services;

public class MixtureFormService(
    IMixtureFormRepository _repository,
    IMapper _mapper,
    AppDbContext _context
) : IMixtureFormService
{
    private static readonly string[] _excludedSearchProperties = ["Id", "FormulaMasterId"];

    public async Task<PagedResultDto<MixtureFormDto>> GetAllAsync(PagedQueryDto query)
    {
        var q = _repository.Query();

        if (query.filter.Any(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)))
        {
            var searchTerms = query.filter
                .Where(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase))
                .Select(f => f.Value)
                .ToList();

            q = q.Where(
                SearchHelper.BuildGlobalSearchPredicate<MixtureForm>(
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

        return new PagedResultDto<MixtureFormDto>
        {
            Items = items.Select(x => new MixtureFormDto
            {
                Id = x.Id,
                FormulaMasterId = x.FormulaMasterId,
                TotalMixture = x.TotalMixture,
                MixtureName = x.MixtureName ?? x.FormulaMaster?.MixtureName ?? string.Empty,
                CreatedDate = x.CreatedDate,
                FinalProductName = x.FormulaMaster?.FinalProduct?.Final_Product ?? string.Empty,
            }),
            TotalCount = total,
            Page = query.page,
            Size = query.size,
        };
    }

    public async Task<MixtureFormDto?> GetByIdAsync(int id)
    {
        var mixtureForm = await _repository.GetByIdAsync(id);
        if (mixtureForm == null)
        {
            return null;
        }

        return new MixtureFormDto
        {
            Id = mixtureForm.Id,
            FormulaMasterId = mixtureForm.FormulaMasterId,
            TotalMixture = mixtureForm.TotalMixture,
            MixtureName = mixtureForm.MixtureName ?? mixtureForm.FormulaMaster?.MixtureName ?? string.Empty,
            CreatedDate = mixtureForm.CreatedDate,
            FinalProductName = mixtureForm.FormulaMaster?.FinalProduct?.Final_Product ?? string.Empty,
        };
    }

    public async Task<MixtureFormDto> CreateAsync(MixtureFormDto dto)
    {
        var formulaMaster = await _context.FormulaMaster
            .Include(x => x.FinalProduct)
            .FirstOrDefaultAsync(x => x.Id == dto.FormulaMasterId);

        if (formulaMaster == null)
        {
            throw new ArgumentException("Formula master not found");
        }

        var mixtureForm = _mapper.Map<MixtureForm>(dto);
        mixtureForm.MixtureName = string.IsNullOrWhiteSpace(dto.MixtureName)
            ? formulaMaster.MixtureName ?? string.Empty
            : dto.MixtureName.Trim();
        mixtureForm.CreatedDate = dto.CreatedDate ?? DateTime.UtcNow;

        await _repository.AddAsync(mixtureForm);
        await _context.SaveChangesAsync();

        return new MixtureFormDto
        {
            Id = mixtureForm.Id,
            FormulaMasterId = mixtureForm.FormulaMasterId,
            TotalMixture = mixtureForm.TotalMixture,
            MixtureName = mixtureForm.MixtureName ?? formulaMaster.MixtureName ?? string.Empty,
            CreatedDate = mixtureForm.CreatedDate,
            FinalProductName = formulaMaster.FinalProduct?.Final_Product ?? string.Empty,
        };
    }
}
