using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class CustomerRepository(AppDbContext _context) : ICustomerRepository
{
    public async Task<IEnumerable<Customer>> GetAllAsync() => await _context.Customer.ToListAsync();

    public async Task<Customer?> GetByIdAsync(int id) => await _context.Customer.FindAsync(id);

    public async Task<Customer> AddAsync(Customer customer)
    {
        await _context.Customer.AddAsync(customer);
        return customer;
    }

    public async Task<Customer?> UpdateAsync(int id, Customer customer)
    {
        var existing = await _context.Customer.FindAsync(id);
        if (existing == null) return null;

        _context.Entry(existing).CurrentValues.SetValues(customer);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var customer = await _context.Customer.FindAsync(id);
        if (customer == null)
            return false;

        _context.Customer.Remove(customer);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Customer?> GetByNameAsync(string name)
    {
        return await _context.Customer.FirstOrDefaultAsync(e => e.Name == name);
    }

    public IQueryable<Customer> Query() =>
        _context.Customer
            .Select(x => new Customer
            {
                Id = x.Id, // Add if needed
                Name = x.Name,
                Address = x.Address,
                Mobile_No = x.Mobile_No,
                Email = x.Email,
                GST_No = x.GST_No,
                IsActive = x.IsActive,
            });
}
