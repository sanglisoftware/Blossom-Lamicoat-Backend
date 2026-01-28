using Api.Application.DTOs;
using Api.Application.Interfaces;
using AutoMapper;
using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Api.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Services;

public class EmployeeService(IEmployeeRepository _repository, IMapper _mapper, AppDbContext _context) : IEmployeeService
{
    private static readonly string[] _excludedSearchProperties = ["ImagePath", "IsActive", "Id", "RoleId",];

    public async Task<PagedResultDto<EmployeeDto>> GetAllAsync(PagedQueryDto query)
    {
        var q = _repository.Query();

        // Apply global search
        if (query.filter.Any(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)))
        {
            var searchTerms = query.filter.Where(f => f.Type.Equals("like", StringComparison.OrdinalIgnoreCase)).Select(f => f.Value).ToList();

            q = q.Where(SearchHelper.BuildGlobalSearchPredicate<Employee>(searchTerms, _excludedSearchProperties));
        }

        var total = await q.CountAsync();

        // Apply sorting
        q = SortHelper.ApplySorting(q, query.sort, s => s.Field, s => s.Dir) ?? q.OrderByDescending(n => n.Id);

        // Pagination
        var skip = (query.page - 1) * query.size;
        var items = await q.Skip(skip).Take(query.size).ToListAsync();

        return new PagedResultDto<EmployeeDto>
        {
            Items = items.Select(_mapper.Map<EmployeeDto>),
            TotalCount = total,
            Page = query.page,
            Size = query.size,
        };
    }

    public async Task<EmployeeDto?> GetByIdAsync(int id)
    {
        var emp = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
        if (emp == null) return null;

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == emp.Username);

        return new EmployeeDto
        {
            Id = emp.Id,
            Username = emp.Username,
            Password = user?.Password ?? "N/A",
            RoleId = emp.RoleId,
            FirstName = emp.FirstName,
            MiddleName = emp.MiddleName,
            LastName = emp.LastName,
            Mobile = emp.Mobile,
            Type = emp.Type,

            ActiveStatus = emp.ActiveStatus,
        };
    }

    public async Task<EmployeeDto> CreateAsync(EmployeeDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Check username exists in either table
            if (await _context.Employees.AnyAsync(e => e.Username == dto.Username) || await _context.Users.AnyAsync(l => l.Username == dto.Username))
            {
                throw new ArgumentException("Username already exists");
            }

            // 2. Create Employee
            var employee = _mapper.Map<Employee>(dto);
            await _repository.AddAsync(employee);
            await _context.SaveChangesAsync();

            // 3. Create Login
            var user = new User
            {
                Username = dto.Username,
                Password = dto.Password, //BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.RoleId,
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return _mapper.Map<EmployeeDto>(employee);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<EmployeeDto?> UpdateAsync(int id, EmployeeDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Update Employee
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            _mapper.Map(dto, existing);
            existing.Id = id;
            await _context.SaveChangesAsync();

            // 2. Update User (Login) password if changed
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == existing.Username);
            if (user != null)
            {
                // Check if password needs updating
                if (!string.IsNullOrEmpty(dto.Password))
                {
                    user.Password = dto.Password; //BCrypt.HashPassword(dto.Password);
                    _context.Users.Update(user);

                    await _context.SaveChangesAsync();
                }
            }

            await transaction.CommitAsync();
            return _mapper.Map<EmployeeDto>(existing);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var employee = await _repository.GetByIdAsync(id);
            if (employee == null) return false;

            // Delete associated user
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == employee.Username);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            // Delete employee
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<EmployeeDto?> UpdateStatusAsync(int id, short isActive)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;
        existing.ActiveStatus = isActive;
        var updated = await _repository.UpdateAsync(id, existing);
        return updated is null ? null : _mapper.Map<EmployeeDto>(updated);
    }
}
