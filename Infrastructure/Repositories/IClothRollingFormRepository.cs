using Api.Domain.Entities;

namespace Api.Infrastructure.Repositories;

public interface IClothRollingFormRepository
{
    IQueryable<ClothRollingForm> Query();
    Task<ClothRollingForm?> GetByIdAsync(int id);
    Task<ClothRollingForm> AddAsync(ClothRollingForm clothRollingForm);
}
