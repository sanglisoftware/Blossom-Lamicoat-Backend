using Api.Domain.Entities;

namespace Api.Infrastructure.Repositories;

public interface ISliderRepository
{
    Task<IEnumerable<Slider>> GetAllAsync();
    IQueryable<Slider> Query();
    Task<Slider?> GetByIdAsync(int id);
    Task AddAsync(Slider slider);
    Task<Slider?> UpdateAsync(int id, Slider updated);
    Task<bool> DeleteAsync(int id);
}
