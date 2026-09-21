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
            .Include(x => x.LaminationForm)
            .Include(x => x.FinalProduct)
            .Select(x => new InspectionForm
            {
                Id = x.Id,
                ManufacturedFabricProductId = x.ManufacturedFabricProductId,
                LaminationFormId = x.LaminationFormId,
                FinalProductId = x.FinalProductId,
                RollNo = x.RollNo ?? string.Empty,
                RollType = x.RollType ?? "Roll",
                GradeId = x.GradeId,
                Mtr = x.Mtr,
                WastageMtr = x.WastageMtr,
                CreatedDate = x.CreatedDate,
                ManufacturedFabricProduct = x.ManufacturedFabricProduct == null
                    ? null
                    : new FproductList
                    {
                        Id = x.ManufacturedFabricProduct.Id,
                        Name = x.ManufacturedFabricProduct.Name ?? string.Empty,
                    },
                Grade = x.Grade == null
                    ? null
                    : new Grade
                    {
                        Id = x.Grade.Id,
                        Name = x.Grade.Name ?? string.Empty,
                    },
                LaminationForm = x.LaminationForm == null ? null : new LaminationForm
                {
                    Id = x.LaminationForm.Id,
                    FinalProductQtyMtr = x.LaminationForm.FinalProductQtyMtr,
                },
                FinalProduct = x.FinalProduct == null ? null : new FinalProduct
                {
                    Id = x.FinalProduct.Id,
                    Final_Product = x.FinalProduct.Final_Product ?? string.Empty,
                },
            });
}
