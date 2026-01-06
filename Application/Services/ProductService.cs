using System.Linq.Expressions;
using Api.Application.Services;
using Api.Application.DTOs;
using Api.Application.Interfaces;
using AutoMapper;
using Api.Domain.Entities;
using Api.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;

    public ProductService(IProductRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    public async Task<PagedResultDto<ProductDto>> GetAllAsync(PagedQueryDto query)
    {
        // Start with includable query
        var queryWithInclude = _repository.QueryWithCategory();

        // 1. Apply global search
        if (query.filter.Any(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)))
        {
            var searchTerms = query.filter
                .Where(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase))
                .Select(f => f.Value)
                .ToList();


            var excludedProperties = new[] { "ImagePaths", "IsActive", "Id", "Color", "CategoryId" };

            var basePredicate = SearchHelper.BuildGlobalSearchPredicate<Product>(searchTerms, excludedProperties);

            Expression<Func<Product, bool>> categoryPredicate = p => false;
            foreach (var term in searchTerms)
            {
                if (string.IsNullOrWhiteSpace(term)) continue;
                var term1 = term;
                var tempPredicate = (Expression<Func<Product, bool>>)(p =>
                    p.Category != null &&
                    EF.Functions.Like(p.Category.Name, $"%{term1}%"));
                categoryPredicate = SearchHelper.CombineOr(categoryPredicate, tempPredicate);
            }

            var finalPredicate = SearchHelper.CombineOr(basePredicate, categoryPredicate);
            queryWithInclude = queryWithInclude.Where(finalPredicate);
        }

        // Apply sorting
        var sortedQuery = SortHelper.ApplySorting<Product, SortDto>(
            queryWithInclude,
            query.sort,
            s => s.Field,
            s => s.Dir
        ) ?? queryWithInclude.OrderBy(p => p.SequenceNo);

        // Get total count
        var total = await sortedQuery.CountAsync();

        // Apply pagination
        var items = (await sortedQuery
            .Skip((query.page - 1) * query.size)
            .Take(query.size)
            .ToListAsync())
            .OrderBy(p => p.SequenceNo);

        // Map to DTO
        var itemsDto = items.Select(p =>
        {
            var dto = _mapper.Map<ProductDto>(p);
            dto.CategoryName = p.Category?.Name;
            return dto;
        });

        return new PagedResultDto<ProductDto>
        {
            Items = itemsDto,
            TotalCount = total,
            Page = query.page,
            Size = query.size
        };
    }
    public async Task<ProductDto?> GetByIdAsync(int id)
  => _mapper.Map<ProductDto>(await _repository.GetByIdAsync(id));

    public async Task<ProductDto> CreateAsync(ProductDto dto)
    {
        // Validate ID is not set for new entities
        if (dto.Id != 0)
        {
            throw new ArgumentException("New products should not have an ID");
        }

        // Create entity without ID
        var entity = new Product
        {
            // Map properties manually to avoid ID
            Name = dto.Name,
            CategoryId = dto.CategoryId,
            SizeId = dto.SizeId,
            SequenceNo = dto.SequenceNo,
            ProductCode = dto.ProductCode,
            Color = dto.Color,
            ImagePaths = dto.ImagePaths,
            VideoUrl = dto.VideoUrl,
            Price = dto.Price,
            Gst = dto.Gst,
            HsnCode = dto.HsnCode,
            ShortDescription = dto.ShortDescription,
            DetailDescription = dto.DetailDescription,
            IsStandalone = dto.IsStandalone,
            IsActive = dto.IsActive
        };

        await _repository.AddAsync(entity);
        return _mapper.Map<ProductDto>(entity);
    }

    public async Task<ProductDto?> UpdateAsync(int id, ProductDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;

        // Always update image paths if provided in DTO
        if (!string.IsNullOrEmpty(dto.ImagePaths))
        {
            existing.ImagePaths = dto.ImagePaths;
        }

        // Update other properties
        existing.Name = dto.Name;
        existing.CategoryId = dto.CategoryId;
        existing.SizeId = dto.SizeId;
        existing.SequenceNo = dto.SequenceNo;
        existing.ProductCode = dto.ProductCode;
        existing.Color = dto.Color;
        existing.VideoUrl = dto.VideoUrl;
        existing.Price = dto.Price;
        existing.Gst = dto.Gst;
        existing.HsnCode = dto.HsnCode;
        existing.ShortDescription = dto.ShortDescription;
        existing.DetailDescription = dto.DetailDescription;
        existing.IsStandalone = dto.IsStandalone;
        existing.IsActive = dto.IsActive;

        var updated = await _repository.UpdateAsync(id, existing);
        return _mapper.Map<ProductDto>(updated);
    }
    public async Task<bool> DeleteAsync(int id) => await _repository.DeleteAsync(id);

    public async Task<List<Product>> ChangeProductStatus(int id, short isActive)
    {
        //get product by category, and change the status of all products under that category
        var products = await _repository.ChangeStatus(id, isActive);
        return products;
    }

    public async Task<ProductDto?> UpdateStatusAsync(int id, short isActive)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;

        // Only update sequence number
        existing.IsActive = isActive;

        var updated = await _repository.UpdateAsync(id, existing);
        return updated is null ? null : _mapper.Map<ProductDto>(updated);
    }

    public async Task<ProductDto?> UpdateSequenceAsync(int id, int sequenceNo)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;

        // Only update sequence number
        existing.SequenceNo = sequenceNo;

        var updated = await _repository.UpdateAsync(id, existing);
        return updated is null ? null : _mapper.Map<ProductDto>(updated);
    }

    public async Task<List<object>> GetAllFeaturedProductsWebsite()
    {
        var q = (await _repository.Query()
        .Where(p => p.IsStandalone == 1) //IsStandalone means IsFetaured?
        .OrderBy(x => x.SequenceNo)
        .ToListAsync())
        .Select(i => new
        {
            id = i.Id,
            img = i.ImagePaths,
            name = i.Name,
            price = i.Price,
            // desc = new List<string>() { "Mango falvour " + i.Id, "Tastes like real mango " + i.Id, "Sweet Delicious " + i.Id }.ToArray()
            desc = new List<string>() { i.ShortDescription ?? "", }.ToArray()
        });
        return _mapper.Map<List<object>>(q);
    }

    public async Task<dynamic> GetAllProductsWebsite(int id, int category, string? search, int pageSize, int page)
    {
        var query = _repository.Query();

        if (category != 0) { query = query.Where(p => p.CategoryId == category); }

        if (!string.IsNullOrWhiteSpace(search)) { query = query.Where(p => ((p == null) ? "" : p.Name.ToLower()).Contains(search.ToLower())); }

        var total = await query.CountAsync();

        if (pageSize == 0) pageSize = 10;
        if (page == 0) page = 1;
        int skip = (page - 1) * pageSize;
        query = query.Skip(skip).Take(pageSize);


        object result;

        if (id != 0)
        {
            var p = await query.Include(x => x.Category).Where(x => x.Id == id).SingleAsync();
            result = new
            {
                img = p.ImagePaths,
                name = p.Name,
                price = p.Price,
                category = p.Category?.Name,
                // desc = new[] { $"Mango flavour {p.Id}", $"Tastes like real mango {p.Id}", $"Sweet Delicious {p.Id}" },
                desc = new List<string>() { p.ShortDescription ?? "", }.ToArray(),
                sd = p.ShortDescription,
                ld = p.DetailDescription,
                sku = p.HsnCode,
            };
        }
        else
        {
            var products = await query.ToListAsync();
            result = new
            {
                products = products.Select(p => new
                {
                    id = p.Id,
                    img = p.ImagePaths,
                    name = p.Name,
                    price = p.Price,
                    // desc = new[] { $"Mango flavour {p.Id}", $"Tastes like real mango {p.Id}", $"Sweet Delicious {p.Id}" },
                    desc = new List<string>() { p.ShortDescription ?? "", }.ToArray()
                }),
                total
            };
        }

        return result;
    }
    public async Task<List<object>> GetRelatedProductsWebsite(int productId)
    {
        var products = await _repository.Query().Where(x => x.Id != productId).OrderBy(r => Guid.NewGuid()).Take(10)
            .Select(i => new
            {
                id = i.Id,
                img = i.ImagePaths,
                name = i.Name,
                price = i.Price,
                desc = new List<string>() { i.ShortDescription ?? "", }.ToArray()
            }).ToListAsync();

        return _mapper.Map<List<object>>(products);
    }
}