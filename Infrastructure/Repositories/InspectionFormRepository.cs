using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class InspectionFormRepository(AppDbContext _context) : IInspectionFormRepository
{
    public async Task<IEnumerable<InspectionForm>> GetAllAsync() =>
        await Query().ToListAsync();

    public async Task<InspectionForm?> GetByIdAsync(int id) =>
        await Query().FirstOrDefaultAsync(x => x.Id == id);

    public async Task<InspectionForm> AddAsync(InspectionForm inspectionForm)
    {
        await _context.InspectionForms.AddAsync(inspectionForm);
        return inspectionForm;
    }

    public IQueryable<InspectionForm> Query() =>
        _context.InspectionForms
            .Include(x => x.ManufacturedFabricProduct)
            .Include(x => x.Grade)
            .Select(x => new InspectionForm
            {
                Id = x.Id,
                ManufacturedFabricProductId = x.ManufacturedFabricProductId,
                GradeId = x.GradeId,
                Mtr = x.Mtr,
                WastageMtr = x.WastageMtr,
                CreatedDate = x.CreatedDate,
                ManufacturedFabricProduct = x.ManufacturedFabricProduct == null
                    ? null
                    : new FproductList
                    {
                        Id = x.ManufacturedFabricProduct.Id,
                        Name = x.ManufacturedFabricProduct.Name,
                    },
                Grade = x.Grade == null
                    ? null
                    : new Grade
                    {
                        Id = x.Grade.Id,
                        Name = x.Grade.Name,
                    },
            });
}
