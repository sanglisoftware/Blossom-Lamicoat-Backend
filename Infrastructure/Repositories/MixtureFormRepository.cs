using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class MixtureFormRepository(AppDbContext _context) : IMixtureFormRepository
{
    public async Task<IEnumerable<MixtureForm>> GetAllAsync() =>
        await _context.MixtureForms
            .Include(x => x.FormulaMaster)
            .ThenInclude(x => x!.FinalProduct)
            .ToListAsync();

    public async Task<MixtureForm?> GetByIdAsync(int id) =>
        await _context.MixtureForms
            .Include(x => x.FormulaMaster)
            .ThenInclude(x => x!.FinalProduct)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<MixtureForm> AddAsync(MixtureForm mixtureForm)
    {
        await _context.MixtureForms.AddAsync(mixtureForm);
        return mixtureForm;
    }

    public IQueryable<MixtureForm> Query() =>
        _context.MixtureForms
            .Include(x => x.FormulaMaster)
            .ThenInclude(x => x.FinalProduct)
            .Select(x => new MixtureForm
            {
                Id = x.Id,
                FormulaMasterId = x.FormulaMasterId,
                TotalMixture = x.TotalMixture,
                MixtureName = x.MixtureName,
                CreatedDate = x.CreatedDate,
                FormulaMaster = x.FormulaMaster == null
                    ? null
                    : new FormulaMaster
                    {
                        Id = x.FormulaMaster.Id,
                        FinalProductId = x.FormulaMaster.FinalProductId,
                        FinalProduct = x.FormulaMaster.FinalProduct == null
                            ? null
                            : new FinalProduct
                            {
                                Id = x.FormulaMaster.FinalProduct.Id,
                                Final_Product = x.FormulaMaster.FinalProduct.Final_Product,
                            },
                    },
            });
}
