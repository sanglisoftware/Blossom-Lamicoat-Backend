using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class EmployeeRepository(AppDbContext _context) : IEmployeeRepository
{
    public async Task<IEnumerable<Employee>> GetAllAsync() => await _context.Employees.ToListAsync();

    public async Task<Employee?> GetByIdAsync(int id) => await _context.Employees.FindAsync(id);

    public async Task<Employee> AddAsync(Employee employee)
    {
        await _context.Employees.AddAsync(employee);
        return employee;
    }

    public async Task<Employee?> UpdateAsync(int id, Employee employee)
    {
        var existing = await _context.Employees.FindAsync(id);
        if (existing == null) return null;

        _context.Entry(existing).CurrentValues.SetValues(employee);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
            return false;

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Employee?> GetByUsernameAsync(string username)
    {
        return await _context.Employees.FirstOrDefaultAsync(e => e.Username == username);
    }

    public IQueryable<Employee> Query() =>
        _context
            .Employees.Include(e => e.Role) // Ensure Role is loaded
            .Select(x => new Employee
            {
                Id = x.Id, // Add if needed
                FirstName = x.FirstName,
                MiddleName = x.MiddleName,
                LastName = x.LastName,
                Mobile = x.Mobile,
                ActiveStatus = x.ActiveStatus,
                RoleValue = x.Role == null ? null : x.Role.RoleValue, // Now accessible
                Username = x.Username, // Add if searching
            });
}
