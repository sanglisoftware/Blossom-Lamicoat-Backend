using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class ClothRollingFormRepository(AppDbContext _context) : IClothRollingFormRepository
{
    public IQueryable<ClothRollingForm> Query() =>
        _context.ClothRollingForms.Select(x => new ClothRollingForm
        {
            Id = x.Id,
            ProductName = x.ProductName,
            BatchNo = x.BatchNo,
            RollMtr = x.RollMtr,
            DefectMtr = x.DefectMtr,
            CheckerName = x.CheckerName,
            IsActive = x.IsActive,
            CreatedDate = x.CreatedDate,
        });

    public async Task<ClothRollingForm?> GetByIdAsync(int id) =>
        await Query().FirstOrDefaultAsync(x => x.Id == id);

    public async Task<ClothRollingForm> AddAsync(ClothRollingForm clothRollingForm)
    {
        await _context.ClothRollingForms.AddAsync(clothRollingForm);
        return clothRollingForm;
    }
}
