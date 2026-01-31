using Api.Domain.Entities;

namespace Api.Infrastructure.Repositories;

public interface IColourRepository
{
    IQueryable<Colour> Query();
    Task<IEnumerable<Colour>> GetAllAsync();
    Task<Colour?> GetByIdAsync(int id);
    Task<Colour> AddAsync(Colour colour);
    Task<Colour?> UpdateAsync(int id, Colour colour);
    Task<bool> DeleteAsync(int id);
}
