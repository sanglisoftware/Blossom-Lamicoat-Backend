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

public class GalleryService(IGalleryRepository _repository, IMapper _mapper, IGalleryFilterService _galleryFilterService, AppDbContext _db) : IGalleryService
{
    private static readonly string[] _excludedSearchProperties = ["ImagePath", "IsActive", "Id"];
    public async Task<PagedResultDto<GalleryDto>> GetAllAsync(PagedQueryDto query)
    {
        var q = _repository.Query();

        // 1. Apply global search
        if (query.filter.Any(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)))
        {
            var searchTerms = query.filter.Where(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)).Select(f => f.Value).ToList();

            q = q.Where(SearchHelper.BuildGlobalSearchPredicate<Gallery>(searchTerms, _excludedSearchProperties));
        }

        // 2. Get total count
        var total = await q.CountAsync();

        // 3. Apply sorting
        q = SortHelper.ApplySorting(q, query.sort, s => s.Field, s => s.Dir) ?? q.OrderBy(c => c.SequenceNo);

        // 4. Pagination and Eager load GalleryFilter
        var skip = (query.page - 1) * query.size;
        var items = await q.Include(g => g.GalleryFilter)  // Eager load GalleryFilter
                            .Skip(skip)
                            .Take(query.size)
                            .ToListAsync();

        // 5. Map and return
        var itemsDto = items.Select(g =>
        {
            var dto = _mapper.Map<GalleryDto>(g);
            dto.FilterName = g.GalleryFilter?.FilterValue ?? string.Empty; // Ensure it doesn't throw null exception
            return dto;
        }).ToList();

        return new PagedResultDto<GalleryDto>
        {
            Items = itemsDto,  // Return itemsDto after mapping
            TotalCount = total,
            Page = query.page,
            Size = query.size,
        };
    }


    public async Task<GalleryDto?> GetByIdAsync(int id) => _mapper.Map<GalleryDto>(await _repository.GetByIdAsync(id));
    public async Task<GalleryDto> CreateAsync(GalleryDto dto)
    {
        var entity = _mapper.Map<Gallery>(dto);
        await _repository.AddAsync(entity);
        return _mapper.Map<GalleryDto>(entity);
    }

    public async Task<GalleryDto?> UpdateAsync(int id, GalleryDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;

        // Only update ImagePath if a new one is provided
        if (string.IsNullOrWhiteSpace(dto.ImagePath)) dto.ImagePath = existing.ImagePath;

        var updated = await _repository.UpdateAsync(id, _mapper.Map<Gallery>(dto));

        if (updated is null)
        {
            return null;
        }

        return updated is null ? null : _mapper.Map<GalleryDto>(updated);
    }

    public async Task<GalleryDto?> UpdateSequenceAsync(int id, int sequenceNo)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;

        // Only update sequence number
        existing.SequenceNo = sequenceNo;

        var updated = await _repository.UpdateAsync(id, existing);
        return updated is null ? null : _mapper.Map<GalleryDto>(updated);
    }

    public async Task<GalleryDto?> UpdateStatusAsync(int id, short isActive)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;

        // Change status of collection
        existing.IsActive = isActive;

        var updated = await _repository.UpdateAsync(id, existing);

        if (updated is null)
        {
            return null;
        }

        return updated is null ? null : _mapper.Map<GalleryDto>(updated);
    }

    public async Task<object> GetGalleryForWebsite()
    {
        // Fetch all gallery items and their corresponding GalleryFilter
        var galleryItems = await _repository.GetAllAsync();

        // Fetch all gallery filters
        var galleryFilters = await _galleryFilterService.GetAllAsync();

        // Join gallery items with gallery filters based on FilterId
        var joinedItems = from gallery in galleryItems
                          join filter in galleryFilters on gallery.FilterId equals filter.Id into filterGroup
                          from filter in filterGroup.DefaultIfEmpty()
                          select new
                          {
                              gallery,
                              galleryFilter = filter
                          };

        // Group gallery items by FilterValue (Handle null values)
        var filters = joinedItems
            .Where(j => !string.IsNullOrEmpty(j.galleryFilter?.FilterValue))
            .GroupBy(j => j.galleryFilter?.FilterValue)
            .Select(g => new
            {
                filter = $".{g.Key?.ToLower() ?? "unknown"}",
                title = g.Key ?? "Unknown"
            })
            .ToList();

        // Add a default "Unknown" filter for items with null FilterValue
        var unknownFilters = joinedItems
            .Where(j => string.IsNullOrEmpty(j.galleryFilter?.FilterValue))
            .Select(j => new
            {
                filter = ".unknown",
                title = "Unknown"
            })
            .Distinct()
            .ToList();

        filters.AddRange(unknownFilters);

        // Format gallery data with associated filters
        var data = joinedItems
            .Select(j => new
            {
                img = j.gallery.ImagePath,
                filter = j.galleryFilter?.FilterValue?.ToLower() ?? "unknown",
                title = j.gallery.Title
            })
            .ToList();


        return new
        {
            filters = filters,
            data = data
        };
    }

    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}
