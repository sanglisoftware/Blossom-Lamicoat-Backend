using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Api.Infrastructure.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories
{
    public class EnquiryRepository : IEnquiryRepository
    {
        private readonly AppDbContext _context;

        public EnquiryRepository(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<Enquiry> Query()
        {
            return _context.Enquiry.AsQueryable();
        }

        public async Task<IEnumerable<Enquiry>> GetAllAsync()
        {
            return await _context.Enquiry.ToListAsync();
        }

        public async Task<Enquiry?> GetByIdAsync(int id)
        {
            return await _context.Enquiry.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Enquiry> AddAsync(Enquiry enquiry)
        {
            _context.Enquiry.Add(enquiry);
            await _context.SaveChangesAsync();
            return enquiry;
        }

        public async Task<Enquiry?> UpdateAsync(int id, Enquiry enquiry)
        {
            var existing = await _context.Enquiry.FirstOrDefaultAsync(e => e.Id == id);
            if (existing == null) return null;

            _context.Entry(existing).CurrentValues.SetValues(enquiry);
            existing.Id = id;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var enquiry = await _context.Enquiry.FirstOrDefaultAsync(e => e.Id == id);
            if (enquiry == null) return false;

            _context.Enquiry.Remove(enquiry);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

