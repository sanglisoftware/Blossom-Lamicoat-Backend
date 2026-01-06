using Api.Application.DTOs;
using Api.Application.Interfaces;
using AutoMapper;
using Api.Domain.Entities;
using Api.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Api.Application.Services
{
    public class NewsService(INewsRepository _newsRepo, IMapper _mapper) : INewsService
    {//, AppDbContext _db
        public async Task<NewsCreateDto> AddNewsAsync(NewsCreateDto dto)
        {
            var entity = _mapper.Map<News>(dto);
            await _newsRepo.AddAsync(entity);
            return _mapper.Map<NewsCreateDto>(entity);
        }

        public async Task<PagedResultDto<NewsCreateDto>> GetAllAsync(PagedQueryDto query)
        {
            var q = _newsRepo.Query();

            // Apply global search
            if (query.filter.Any(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)))
            {
                var searchTerms = query
                    .filter.Where(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase))
                    .Select(f => f.Value)
                    .ToList();

                q = q.Where(
                    SearchHelper.BuildGlobalSearchPredicate<News>(
                        searchTerms,
                        ["Img", "IsActive", "Id"]
                    )
                );
            }

            var total = await q.CountAsync();

            // Apply sorting
            q =
                SortHelper.ApplySorting<News, SortDto>(q, query.sort, s => s.Field, s => s.Dir)
                ?? q.OrderByDescending(n => n.Id);

            // Pagination
            var skip = (query.page - 1) * query.size;
            var items = await q.Skip(skip).Take(query.size).ToListAsync();

            return new PagedResultDto<NewsCreateDto>
            {
                Items = items.Select(_mapper.Map<NewsCreateDto>),
                TotalCount = total,
                Page = query.page,
                Size = query.size,
            };
        }

        public async Task<NewsCreateDto?> GetByIdAsync(int id) =>
            _mapper.Map<NewsCreateDto>(await _newsRepo.GetByIdAsync(id));

        public async Task<NewsCreateDto?> UpdateAsync(int id, NewsCreateDto dto)
        {
            var existing = await _newsRepo.GetByIdAsync(id);
            if (existing == null)
                return null;

            // Preserve existing image if not provided in update
            if (string.IsNullOrEmpty(dto.Img))
                dto.Img = existing.Img;

            var entity = _mapper.Map<News>(dto);
            var updated = await _newsRepo.UpdateAsync(id, entity);
            return updated == null ? null : _mapper.Map<NewsCreateDto>(updated);
        }

        public async Task<NewsCreateDto?> UpdateStatusAsync(int id, short isActive)
        {
            var existing = await _newsRepo.GetByIdAsync(id);
            if (existing == null)
                return null;

            existing.IsActive = isActive;
            var updated = await _newsRepo.UpdateAsync(id, existing);
            return updated == null ? null : _mapper.Map<NewsCreateDto>(updated);
        }

        public async Task<bool> DeleteAsync(int id) => await _newsRepo.DeleteAsync(id);
    }

}
