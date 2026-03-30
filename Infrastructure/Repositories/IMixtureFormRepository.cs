using Api.Domain.Entities;

namespace Api.Infrastructure.Repositories;

public interface IMixtureFormRepository
{
    IQueryable<MixtureForm> Query();
    Task<IEnumerable<MixtureForm>> GetAllAsync();
    Task<MixtureForm?> GetByIdAsync(int id);
    Task<MixtureForm> AddAsync(MixtureForm mixtureForm);
}
