using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class ClothRollingFormRepository(AppDbContext _context) : IClothRollingFormRepository
{
    public IQueryable<ClothRollingForm> Query() =>
        _context.ClothRollingForms
            .Include(x => x.FabricInward)
                .ThenInclude(x => x!.FGramage)
            .Include(x => x.FabricInward)
                .ThenInclude(x => x!.Colour);

    public async Task<ClothRollingForm?> GetByIdAsync(int id) =>
        await Query().FirstOrDefaultAsync(x => x.Id == id);

    public async Task<ClothRollingForm> AddAsync(ClothRollingForm clothRollingForm)
    {
        await _context.ClothRollingForms.AddAsync(clothRollingForm);
        return clothRollingForm;
    }
}
