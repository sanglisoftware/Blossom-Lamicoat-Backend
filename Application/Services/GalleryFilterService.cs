using Api.Application.Services;
using Api.Application.DTOs;
using Api.Application.Interfaces;
using AutoMapper;
using Api.Domain.Entities;
using Api.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Api.Application.Services;

public class GalleryFilterService : IGalleryFilterService
{
    private readonly IGalleryFilterRepository _repository;
    private readonly IMapper _mapper;

    public GalleryFilterService(IGalleryFilterRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    private static readonly string[] _excludedSearchProperties = { "Id" };
    public async Task<PagedResultDto<GalleryFilterDto>> GetAllAsync(PagedQueryDto query)
    {
        var q = _repository.Query();

        // 1. Apply global search
        if (query.filter.Any(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)))
        {
            var searchTerms = query
                .filter.Where(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase))
                .Select(f => f.Value)
                .ToList();

            q = q.Where(
                SearchHelper.BuildGlobalSearchPredicate<GalleryFilter>(
                    searchTerms,
                    _excludedSearchProperties
                )
            );
        }

        // 2. Get total count
        var total = await q.CountAsync();

        // 3. Apply sorting
        q =
            SortHelper.ApplySorting(q, query.sort, s => s.Field, s => s.Dir)
            ?? q.OrderBy(c => c.Id);

        // 4. Pagination
        var skip = (query.page - 1) * query.size;
        var items = await q.Skip(skip).Take(query.size).ToListAsync();

        // 5. Map and return
        return new PagedResultDto<GalleryFilterDto>
        {
            Items = items.Select(_mapper.Map<GalleryFilterDto>),
            TotalCount = total,
            Page = query.page,
            Size = query.size,
        };
    }
    public async Task<IEnumerable<GalleryFilterDto>> GetAllAsync() =>
        _mapper.Map<IEnumerable<GalleryFilterDto>>(await _repository.GetAllAsync());

    public async Task<GalleryFilterDto?> GetByIdAsync(int id)
    {
        var Filter = await _repository.GetByIdAsync(id);
        return Filter is null ? null : _mapper.Map<GalleryFilterDto>(Filter);
    }
    public async Task<GalleryFilterDto> CreateAsync(GalleryFilterDto dto)
    {
        var Filter = _mapper.Map<GalleryFilter>(dto);
        var created = await _repository.CreateAsync(Filter);
        return _mapper.Map<GalleryFilterDto>(created);
    }

    public async Task<GalleryFilterDto?> UpdateAsync(int id, GalleryFilterDto dto)
    {
        var updated = await _repository.UpdateAsync(id, _mapper.Map<GalleryFilter>(dto));
        return updated is null ? null : _mapper.Map<GalleryFilterDto>(updated);
    }

    public async Task<bool> DeleteAsync(int id) => await _repository.DeleteAsync(id);
}