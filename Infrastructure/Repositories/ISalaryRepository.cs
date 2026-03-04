using Api.Domain.Entities;

namespace Api.Infrastructure.Repositories;

public interface ISalaryRepository
{
    IQueryable<Salary> Query();
    Task<IEnumerable<Salary>> GetAllAsync();
    Task<Salary?> GetByIdAsync(int id);
    Task<Salary> AddAsync(Salary salary);
}
