using Api.Domain.Entities;

namespace Api.Infrastructure.Repositories;

public interface IChemicalInwardRepository
{
    IQueryable<ChemicalInward> Query();
    Task<IEnumerable<ChemicalInward>> GetAllAsync();
    Task<ChemicalInward?> GetByIdAsync(int id);
    Task<ChemicalInward> AddAsync(ChemicalInward chemicalinward);
    Task<ChemicalInward?> UpdateAsync(int id, ChemicalInward chemicalinward);
    Task<bool> DeleteAsync(int id);
}
