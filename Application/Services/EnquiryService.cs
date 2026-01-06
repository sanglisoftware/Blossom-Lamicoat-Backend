using Api.Application.DTOs;
using Api.Application.Interfaces;
using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Api.Infrastructure.Repositories;
using Api.Infrastructure.Repositories.Interface;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Api.Application.Services
{
    public class EnquiryService(IEnquiryRepository _repository, IMapper _mapper, AppDbContext _context) : IEnquiryService
    {
        private static readonly string[] _excludedSearchProperties = new[] {"FeedBack", };


        public async Task<PagedResultDto<EnquiryResponseDto>> GetAllAsync(PagedQueryDto query)
        {
            var q = _repository.Query();

            // Apply global search
            if (query.filter.Any(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)))
            {
                var searchTerms = query.filter.Where(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)).Select(f => f.Value).ToList();

                q = q.Where(SearchHelper.BuildGlobalSearchPredicate<Enquiry>(searchTerms, _excludedSearchProperties));
            }

            var total = await q.CountAsync();

            // Apply sorting
            q = SortHelper.ApplySorting(q, query.sort, s => s.Field, s => s.Dir) ?? q.OrderByDescending(n => n.Id);

            // Pagination
            var skip = (query.page - 1) * query.size;
            var items = await q.Skip(skip).Take(query.size).ToListAsync();

            return new PagedResultDto<EnquiryResponseDto>
            {
                Items = items.Select(_mapper.Map<EnquiryResponseDto>),
                TotalCount = total,
                Page = query.page,
                Size = query.size,
            };
        }

        public async Task<EnquiryResponseDto?> GetByIdAsync(int id)
        {
            var enq = await _context.Enquiry.FirstOrDefaultAsync(e => e.Id == id);
            if (enq == null) return null;

            /// Get Employee details
            //var createdByUsername = await _context.Users.FirstOrDefaultAsync(x => x.Username == enq.);

            return new EnquiryResponseDto
            {
                Id = enq.Id,
                Name = enq.Name,
                //Product = user?.Password ?? "N/A",
                Product = enq.Product,
                MobileNumber = enq.MobileNumber,
                PrimaryDiscussion = enq.PrimaryDiscussion,
                status = enq.status,
                FollowupDate = enq.FollowupDate,
                FeedBack = enq.FeedBack,

            };
        }

        public async Task<EnquiryResponseDto> CreateAsync(EnquiryResponseDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                //// 1. Check username exists in either table
                //if (await _context.Enquiry.AnyAsync(e => e.Username == dto.Username) || await _context.Users.AnyAsync(l => l.Username == dto.Username))
                //{
                //    throw new ArgumentException("Username already exists");
                //}

                var enquiry = _mapper.Map<Enquiry>(dto);
                await _repository.AddAsync(enquiry);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return _mapper.Map<EnquiryResponseDto>(enquiry);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<EnquiryResponseDto?> UpdateAsync(int id, EnquiryResponseDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Update Employee
                var existing = await _repository.GetByIdAsync(id);
                if (existing == null) return null;

                _mapper.Map(dto, existing);
                existing.Id = id;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return _mapper.Map<EnquiryResponseDto>(existing);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var enquiry = await _repository.GetByIdAsync(id);
                if (enquiry == null) return false;


                // Delete enquiry
                _context.Enquiry.Remove(enquiry);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<EnquiryResponseDto?> UpdateStatusAsync(int id, short isActive)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;
            existing.status = false;
            var updated = await _repository.UpdateAsync(id, existing);
            return updated is null ? null : _mapper.Map<EnquiryResponseDto>(updated);
        }
    }
}
