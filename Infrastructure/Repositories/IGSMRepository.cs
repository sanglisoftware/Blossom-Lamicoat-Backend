using Api.Domain.Entities;

namespace Api.Infrastructure.Repositories;

public interface IGSMRepository
{
    IQueryable<GSM> Query();
    Task<IEnumerable<GSM>> GetAllAsync();
    Task<GSM?> GetByIdAsync(int id);
    Task<GSM> AddAsync(GSM gsm);
    Task<GSM?> UpdateAsync(int id, GSM gsm);
    Task<bool> DeleteAsync(int id);
}
