using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class SalaryRepository(AppDbContext _context) : ISalaryRepository
{
    public async Task<IEnumerable<Salary>> GetAllAsync() =>
        await _context.Salaries.Include(s => s.Employee).ToListAsync();

    public async Task<Salary?> GetByIdAsync(int id) =>
        await _context.Salaries.Include(s => s.Employee).FirstOrDefaultAsync(s => s.Id == id);

    public async Task<Salary> AddAsync(Salary salary)
    {
        await _context.Salaries.AddAsync(salary);
        return salary;
    }

    public IQueryable<Salary> Query() =>
        _context.Salaries.Include(s => s.Employee).Select(x => new Salary
        {
            Id = x.Id,
            EmployeeId = x.EmployeeId,
            Type = x.Type,
            Attendance = x.Attendance,
            ExtraHours = x.ExtraHours,
            TotalLate = x.TotalLate,
            HalfDay = x.HalfDay,
            TotalSalary = x.TotalSalary,
            CreatedDate = x.CreatedDate,
            Employee = x.Employee == null
                ? null
                : new Employee
                {
                    Id = x.Employee.Id,
                    FirstName = x.Employee.FirstName,
                    MiddleName = x.Employee.MiddleName,
                    LastName = x.Employee.LastName,
                },
        });
}
