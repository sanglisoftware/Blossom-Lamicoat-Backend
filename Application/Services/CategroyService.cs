using Api.Application.DTOs;
using Api.Application.Interfaces;
using AutoMapper;
using Api.Domain.Entities;
using Api.Infrastructure.Repositories;

namespace Api.Application.Services;

using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class CategoryService(ICategoryRepository _repository, IMapper _mapper, IProductService _productService, AppDbContext _db) : ICategoryService
{
    private static readonly string[] _excludedSearchProperties = ["ImagePath", "IsActive", "Id"];

    public async Task<PagedResultDto<CategoryDto>> GetAllAsync(PagedQueryDto query)
    {
        var q = _repository.Query();

        // 1. Apply global search
        if (query.filter.Any(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)))
        {
            var searchTerms = query.filter.Where(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)).Select(f => f.Value).ToList();

            q = q.Where(SearchHelper.BuildGlobalSearchPredicate<Category>(searchTerms, _excludedSearchProperties));
        }

        // 2. Get total count
        var total = await q.CountAsync();

        // 3. Apply sorting
        q = SortHelper.ApplySorting(q, query.sort, s => s.Field, s => s.Dir) ?? q.OrderBy(c => c.SequenceNo);

        // 4. Pagination
        var skip = (query.page - 1) * query.size;
        var items = await q.Skip(skip).Take(query.size).ToListAsync();

        // 5. Map and return
        return new PagedResultDto<CategoryDto>
        {
            Items = items.Select(_mapper.Map<CategoryDto>),
            TotalCount = total,
            Page = query.page,
            Size = query.size,
        };
    }

    public async Task<CategoryDto?> GetByIdAsync(int id) => _mapper.Map<CategoryDto>(await _repository.GetByIdAsync(id));

    public async Task<CategoryDto> CreateAsync(CategoryDto dto)
    {
        var entity = _mapper.Map<Category>(dto);
        await _repository.AddAsync(entity);
        return _mapper.Map<CategoryDto>(entity);
    }

    public async Task<CategoryDto?> UpdateAsync(int id, CategoryDto dto)
    {
        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            // Only update ImagePath if a new one is provided
            if (string.IsNullOrWhiteSpace(dto.ImagePath)) dto.ImagePath = existing.ImagePath;

            var updated = await _repository.UpdateAsync(id, _mapper.Map<Category>(dto));

            if (updated is null)
            {
                await transaction.RollbackAsync();
                return null;
            }

            await _productService.ChangeProductStatus(id, (short)dto.IsActive);
            await transaction.CommitAsync();
            return updated is null ? null : _mapper.Map<CategoryDto>(updated);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<CategoryDto?> UpdateSequenceAsync(int id, int sequenceNo)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;

        // Only update sequence number
        existing.SequenceNo = sequenceNo;

        var updated = await _repository.UpdateAsync(id, existing);
        return updated is null ? null : _mapper.Map<CategoryDto>(updated);
    }

    public async Task<CategoryDto?> UpdateStatusAsync(int id, short isActive)
    {
        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            // Change status of collection
            existing.IsActive = isActive;

            var updated = await _repository.UpdateAsync(id, existing);

            if (updated is null)
            {
                await transaction.RollbackAsync();
                return null;
            }

            await _productService.ChangeProductStatus(id, isActive);
            await transaction.CommitAsync();
            return updated is null ? null : _mapper.Map<CategoryDto>(updated);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id) => await _repository.DeleteAsync(id);

    public async Task<List<object>> GetAllCategoriesWebsite()
    {
        var q = await _repository.Query().OrderBy(x => x.SequenceNo).ToListAsync();
        return _mapper.Map<List<object>>(q.Select(c => new { c.Id, img = c.ImagePath, title = c.Name, }));
    }
}