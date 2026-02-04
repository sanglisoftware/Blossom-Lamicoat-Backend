using Api.Domain.Entities;

namespace Api.Infrastructure.Repositories;

public interface IFGramageRepository
{
    IQueryable<FGramage> Query();
    Task<IEnumerable<FGramage>> GetAllAsync();
    Task<FGramage?> GetByIdAsync(int id);
    Task<FGramage> AddAsync(FGramage fgramage);
    Task<FGramage?> UpdateAsync(int id, FGramage fgramage);
    Task<bool> DeleteAsync(int id);
}
