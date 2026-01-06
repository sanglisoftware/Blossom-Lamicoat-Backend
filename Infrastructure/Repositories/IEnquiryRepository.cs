using Api.Domain.Entities;

namespace Api.Infrastructure.Repositories.Interface
{
    public interface IEnquiryRepository
    {
        IQueryable<Enquiry> Query();

        Task<IEnumerable<Enquiry>> GetAllAsync();

        Task<Enquiry?> GetByIdAsync(int id);

        Task<Enquiry> AddAsync(Enquiry enquiry);

        Task<Enquiry?> UpdateAsync(int id, Enquiry enquiry);

        Task<bool> DeleteAsync(int id);
    }
}
