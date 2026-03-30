using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class LaminationFormRepository(AppDbContext _context) : ILaminationFormRepository
{
    public async Task<LaminationForm?> GetByIdAsync(int id) =>
        await Query().FirstOrDefaultAsync(x => x.Id == id);

    public async Task<LaminationForm> AddAsync(LaminationForm laminationForm)
    {
        await _context.Set<LaminationForm>().AddAsync(laminationForm);
        return laminationForm;
    }

    public IQueryable<LaminationForm> Query() =>
        _context.Set<LaminationForm>()
            .Include(x => x.FinalProduct)
            .Include(x => x.ClothRollingForm)
            .Include(x => x.PVC)
            .Include(x => x.Chemical)
            .Include(x => x.Worker)
            .Select(x => new LaminationForm
            {
                Id = x.Id,
                FinalProductId = x.FinalProductId,
                ClothRollingFormId = x.ClothRollingFormId,
                ClothRollBatchNo = x.ClothRollBatchNo,
                PVCMasterId = x.PVCMasterId,
                PVCBatchNo = x.PVCBatchNo,
                PVCQty = x.PVCQty,
                ChemicalId = x.ChemicalId,
                ChemicalQty = x.ChemicalQty,
                Bounding = x.Bounding,
                WorkerId = x.WorkerId,
                Temperature = x.Temperature,
                ProcessTime = x.ProcessTime,
                CreatedDate = x.CreatedDate,
                FinalProduct = x.FinalProduct == null
                    ? null
                    : new FinalProduct
                    {
                        Id = x.FinalProduct.Id,
                        Final_Product = x.FinalProduct.Final_Product,
                    },
                ClothRollingForm = x.ClothRollingForm == null
                    ? null
                    : new ClothRollingForm
                    {
                        Id = x.ClothRollingForm.Id,
                        BatchNo = x.ClothRollingForm.BatchNo,
                    },
                PVC = x.PVC == null
                    ? null
                    : new PVCproductList
                    {
                        Id = x.PVC.Id,
                        Name = x.PVC.Name,
                    },
                Chemical = x.Chemical == null
                    ? null
                    : new Chemical
                    {
                        Id = x.Chemical.Id,
                        Name = x.Chemical.Name,
                    },
                Worker = x.Worker == null
                    ? null
                    : new Employee
                    {
                        Id = x.Worker.Id,
                        FirstName = x.Worker.FirstName,
                        MiddleName = x.Worker.MiddleName,
                        LastName = x.Worker.LastName,
                    },
            });
}
