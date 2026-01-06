using Api.Application.DTOs;
using Api.Application.Interfaces;
using AutoMapper;
using Api.Domain.Entities;
using Api.Infrastructure.Repositories;

namespace Api.Application.Services;

using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class SliderService(ISliderRepository _repository, IMapper _mapper, AppDbContext _db) : ISliderService
{
    private static readonly string[] _excludedSearchProperties = ["ImagePath", "IsActive", "Id"];
    public async Task<PagedResultDto<SliderDto>> GetAllAsync(PagedQueryDto query)
    {
        var q = _repository.Query();

        // 1. Apply global search
        if (query.filter.Any(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)))
        {
            var searchTerms = query.filter.Where(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)).Select(f => f.Value).ToList();

            q = q.Where(SearchHelper.BuildGlobalSearchPredicate<Slider>(searchTerms, _excludedSearchProperties));
        }

        // 2. Get total count
        var total = await q.CountAsync();

        // 3. Apply sorting
        q = SortHelper.ApplySorting(q, query.sort, s => s.Field, s => s.Dir) ?? q.OrderBy(c => c.SequenceNo);

        // 4. Pagination
        var skip = (query.page - 1) * query.size;
        var items = await q.Skip(skip).Take(query.size).ToListAsync();

        // 5. Map and return
        return new PagedResultDto<SliderDto>
        {
            Items = items.Select(_mapper.Map<SliderDto>),
            TotalCount = total,
            Page = query.page,
            Size = query.size,
        };
    }
    public async Task<SliderDto> CreateAsync(SliderDto dto)
    {
        var entity = _mapper.Map<Slider>(dto);
        await _repository.AddAsync(entity);
        return _mapper.Map<SliderDto>(entity);
    }

    public async Task<SliderDto?> GetByIdAsync(int id) => _mapper.Map<SliderDto>(await _repository.GetByIdAsync(id));

    public async Task<SliderDto?> UpdateAsync(int id, SliderDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;

        // Only update ImagePath if a new one is provided
        if (string.IsNullOrWhiteSpace(dto.ImagePath)) dto.ImagePath = existing.ImagePath;

        var updated = await _repository.UpdateAsync(id, _mapper.Map<Slider>(dto));
        return updated is null ? null : _mapper.Map<SliderDto>(updated);
    }

    public async Task<SliderDto?> UpdateSequenceAsync(int id, int sequenceNo)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;

        // Only update sequence number
        existing.SequenceNo = sequenceNo;

        var updated = await _repository.UpdateAsync(id, existing);
        return updated is null ? null : _mapper.Map<SliderDto>(updated);
    }

    public async Task<SliderDto?> UpdateStatusAsync(int id, short isActive)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;

        existing.IsActive = isActive;

        var updated = await _repository.UpdateAsync(id, existing);

        return updated is null ? null : _mapper.Map<SliderDto>(updated);
    }
    public async Task<bool> DeleteAsync(int id) => await _repository.DeleteAsync(id);

    public async Task<List<string>> GetSliderImagesForWebsite()
    {
        var sliders = await _repository.GetAllAsync();
        return sliders.Select(s => s.ImagePath).ToList();
    }
}