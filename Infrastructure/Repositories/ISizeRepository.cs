using Api.Domain.Entities;

namespace Api.Infrastructure.Repositories;

public interface ISizeRepository
{
    Task<IEnumerable<Size>> GetAllAsync();
    Task<Size?> GetByIdAsync(int id);
    Task<Size> CreateAsync(Size size);
    Task<Size?> UpdateAsync(int id, Size size);
    Task<bool> DeleteAsync(int id);
}
