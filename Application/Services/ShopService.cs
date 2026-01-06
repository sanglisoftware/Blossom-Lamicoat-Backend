using Api.Application.Services;
using Api.Application.DTOs;
using Api.Application.Interfaces;
using AutoMapper;
using Api.Domain.Entities;
using Api.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

public class ShopService : IShopService
{
    private readonly IShopRepository _repository;
    private readonly IMapper _mapper;
    public ShopService(IShopRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ShopDto> CreateShopAsync(ShopDto dto)
    {
        var entity = _mapper.Map<Shop>(dto);
        await _repository.AddAsync(entity);
        return _mapper.Map<ShopDto>(entity);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        await _repository.DeleteAsync(entity);
        return true;
    }

    public async Task<IEnumerable<ShopDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<ShopDto>>(entities);
    }
    public async Task<ShopDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity is null ? null : _mapper.Map<ShopDto>(entity);
    }

    public async Task<ShopDto?> UpdateAsync(int id, ShopDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;

        // Manually update properties, except Id
        existing.Title = dto.Title;
        existing.Href = dto.Href;
        existing.Mobile = dto.Mobile;
        existing.Address = dto.Address;
        existing.Latitude = dto.Latitude;
        existing.Longitude = dto.Longitude;
        existing.IsActive = dto.IsActive;

        await _repository.UpdateAsync(existing);
        return _mapper.Map<ShopDto>(existing);
    }

    public async Task<ShopDto?> UpdateStatusAsync(int id, short isActive)
    {
        var shop = await _repository.GetByIdAsync(id);
        if (shop == null) return null;

        shop.IsActive = isActive;
        await _repository.UpdateAsync(shop);

        return _mapper.Map<ShopDto>(shop);
    }

    //Tabulator
    private static readonly string[] _excludedSearchProperties = { "Id" };
    public async Task<PagedResultDto<ShopDto>> GetAllAsync(PagedQueryDto query)
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
                SearchHelper.BuildGlobalSearchPredicate<Shop>(
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
        return new PagedResultDto<ShopDto>
        {
            Items = items.Select(_mapper.Map<ShopDto>),
            TotalCount = total,
            Page = query.page,
            Size = query.size,
        };
    }

    //for website
    public async Task<IEnumerable<ShopResponseDto>> GetAllShopsForWebSite()
    {
        var shops = await _repository.GetAllAsync();

        return shops.Select(shop => new ShopResponseDto
        {
            Id = shop.Id,
            Title = shop.Title,
            Href = shop.Href,
            Mobile = shop.Mobile,
            Address = shop.Address,
            Latitude = shop.Latitude,
            Longitude = shop.Longitude
        });
    }
    //For website
    public async Task<IEnumerable<ShopResponseDto>> GetSearchedShopsForWeb(string? search)
    {
        var shops = await _repository.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var lowerSearch = search.ToLower();

            shops = shops
                .Where(s =>
                    (!string.IsNullOrEmpty(s.Title) && s.Title.ToLower().Contains(lowerSearch)) ||
                    (!string.IsNullOrEmpty(s.Href) && s.Href.ToLower().Contains(lowerSearch)) ||
                    (!string.IsNullOrEmpty(s.Mobile) && s.Mobile.ToLower().Contains(lowerSearch)) ||
                    (!string.IsNullOrEmpty(s.Address) && s.Address.ToLower().Contains(lowerSearch)) ||
                    s.Latitude.ToString().Contains(lowerSearch) ||
                    s.Longitude.ToString().Contains(lowerSearch)
                )
                .ToList();
        }

        return shops.Select(s => new ShopResponseDto
        {
            Id = s.Id,
            Title = s.Title,
            Href = s.Href,
            Mobile = s.Mobile,
            Address = s.Address,
            Latitude = s.Latitude,
            Longitude = s.Longitude
        });
    }
}
