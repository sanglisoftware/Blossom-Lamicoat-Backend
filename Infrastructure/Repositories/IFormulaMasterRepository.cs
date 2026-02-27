using Api.Domain.Entities;

namespace Api.Infrastructure.Repositories;

public interface IFormulaMasterRepository
{
    IQueryable<FormulaMaster> Query();
    Task<IEnumerable<FormulaMaster>> GetAllAsync();
    Task<FormulaMaster?> GetByIdAsync(int id);
    Task<FormulaMaster> AddAsync(FormulaMaster formulamaster);
    Task<FormulaMaster?> UpdateAsync(int id, FormulaMaster formulamaster);
    Task<bool> DeleteAsync(int id);
}
