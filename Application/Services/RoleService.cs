using Api.Application.Services;
using Api.Application.DTOs;
using Api.Application.Interfaces;
using AutoMapper;
using Api.Domain.Entities;
using Api.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Api.Application.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _repository;
    private readonly IMapper _mapper;

    public RoleService(IRoleRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RoleDto>> GetAllAsync() =>
        _mapper.Map<IEnumerable<RoleDto>>(await _repository.GetAllAsync());

    public async Task<RoleDto?> GetByIdAsync(int id)
    {
        var Role = await _repository.GetByIdAsync(id);
        return Role is null ? null : _mapper.Map<RoleDto>(Role);
    }

    public async Task<RoleDto> CreateAsync(RoleDto dto)
    {
        var Role = _mapper.Map<Role>(dto);
        var created = await _repository.CreateAsync(Role);
        return _mapper.Map<RoleDto>(created);
    }

    public async Task<RoleDto?> UpdateAsync(int id, RoleDto dto)
    {
        var updated = await _repository.UpdateAsync(id, _mapper.Map<Role>(dto));
        return updated is null ? null : _mapper.Map<RoleDto>(updated);
    }

    public async Task<bool> DeleteAsync(int id) => await _repository.DeleteAsync(id);

    private static readonly string[] _excludedSearchProperties = { "Id" };

    public async Task<PagedResultDto<RoleDto>> GetAllAsync(PagedQueryDto query)
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
                SearchHelper.BuildGlobalSearchPredicate<Role>(
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
        return new PagedResultDto<RoleDto>
        {
            Items = items.Select(_mapper.Map<RoleDto>),
            TotalCount = total,
            Page = query.page,
            Size = query.size,
        };
    }
}
