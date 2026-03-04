using Api.Application.DTOs;
using Api.Application.Interfaces;
using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Api.Infrastructure.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Services;

public class SalaryService(ISalaryRepository _repository, IMapper _mapper, AppDbContext _context) : ISalaryService
{
    private static readonly string[] _excludedSearchProperties = ["Id", "EmployeeId", "Type"];

    public async Task<PagedResultDto<SalaryDto>> GetAllAsync(PagedQueryDto query)
    {
        var q = _repository.Query();

        if (query.filter.Any(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)))
        {
            var searchTerms = query.filter
                .Where(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase))
                .Select(f => f.Value)
                .ToList();

            q = q.Where(SearchHelper.BuildGlobalSearchPredicate<Salary>(searchTerms, _excludedSearchProperties));
        }

        var total = await q.CountAsync();

        q = SortHelper.ApplySorting(q, query.sort, s => s.Field, s => s.Dir) ?? q.OrderByDescending(n => n.Id);

        var skip = (query.page - 1) * query.size;
        var items = await q.Skip(skip).Take(query.size).ToListAsync();

        return new PagedResultDto<SalaryDto>
        {
            Items = items.Select(s => new SalaryDto
            {
                Id = s.Id,
                EmployeeId = s.EmployeeId,
                Type = s.Type,
                Attendance = s.Attendance,
                ExtraHours = s.ExtraHours,
                TotalLate = s.TotalLate,
                HalfDay = s.HalfDay,
                TotalSalary = s.TotalSalary,
                CreatedDate = s.CreatedDate,
                EmployeeName = s.Employee == null
                    ? string.Empty
                    : string.Join(" ", new[] { s.Employee.FirstName, s.Employee.MiddleName, s.Employee.LastName }
                        .Where(n => !string.IsNullOrWhiteSpace(n))),
            }),
            TotalCount = total,
            Page = query.page,
            Size = query.size,
        };
    }

    public async Task<SalaryDto?> GetByIdAsync(int id)
    {
        var salary = await _repository.GetByIdAsync(id);
        if (salary == null)
        {
            return null;
        }

        return new SalaryDto
        {
            Id = salary.Id,
            EmployeeId = salary.EmployeeId,
            Type = salary.Type,
            Attendance = salary.Attendance,
            ExtraHours = salary.ExtraHours,
            TotalLate = salary.TotalLate,
            HalfDay = salary.HalfDay,
            TotalSalary = salary.TotalSalary,
            CreatedDate = salary.CreatedDate,
            EmployeeName = salary.Employee == null
                ? string.Empty
                : string.Join(" ", new[] { salary.Employee.FirstName, salary.Employee.MiddleName, salary.Employee.LastName }
                    .Where(n => !string.IsNullOrWhiteSpace(n))),
        };
    }

    public async Task<SalaryDto> CreateAsync(SalaryDto dto)
    {
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == dto.EmployeeId);
        if (employee == null)
        {
            throw new ArgumentException("Employee not found");
        }

        var salary = _mapper.Map<Salary>(dto);
        salary.Type = employee.Type;
        salary.CreatedDate = dto.CreatedDate ?? DateTime.UtcNow;

        await _repository.AddAsync(salary);
        await _context.SaveChangesAsync();

        return new SalaryDto
        {
            Id = salary.Id,
            EmployeeId = salary.EmployeeId,
            Type = salary.Type,
            Attendance = salary.Attendance,
            ExtraHours = salary.ExtraHours,
            TotalLate = salary.TotalLate,
            HalfDay = salary.HalfDay,
            TotalSalary = salary.TotalSalary,
            CreatedDate = salary.CreatedDate,
            EmployeeName = string.Join(" ", new[] { employee.FirstName, employee.MiddleName, employee.LastName }
                .Where(n => !string.IsNullOrWhiteSpace(n))),
        };
    }
}
