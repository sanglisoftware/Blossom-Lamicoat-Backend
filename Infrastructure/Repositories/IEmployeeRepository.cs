using Api.Domain.Entities;

namespace Api.Infrastructure.Repositories;

public interface IEmployeeRepository
{
    IQueryable<Employee> Query();
    Task<IEnumerable<Employee>> GetAllAsync();
    Task<Employee?> GetByIdAsync(int id);
    Task<Employee> AddAsync(Employee employee);
    Task<Employee?> UpdateAsync(int id, Employee employee);
    Task<bool> DeleteAsync(int id);
}
