using Api.Application.DTOs;
using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.API.EndPoints.Inventory;

public static class StockManagementEndpoints
{
    public static void MapStockManagementEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/stock-management").RequireAuthorization();

        group.MapGet("/", async (AppDbContext db) =>
        {
            var chemicals = await db.Chemical.AsNoTracking()
                .Where(x => x.IsActive == 1)
                .OrderBy(x => x.Name)
                .Select(x => new { x.Id, x.Name })
                .ToListAsync();

            var inward = await db.ChemicalInward.AsNoTracking()
                .Where(x => x.IsActive == null || x.IsActive == 1)
                .Select(x => new
                {
                    x.ChemicalMasterId,
                    x.Qty,
                    Unit = x.UnitOfMeasurement != null ? x.UnitOfMeasurement.Name : string.Empty
                })
                .ToListAsync();

            var formulaChemicals = await db.FormulaChemicalTransaction.AsNoTracking()
                .Select(x => new { x.FormulaMasterId, x.ChemicalMasterId, x.Qty })
                .ToListAsync();
            var mixtures = await db.MixtureForms.AsNoTracking()
                .Select(x => new { x.FormulaMasterId, x.TotalMixture })
                .ToListAsync();
            var returns = await db.ChemicalStockReturns.AsNoTracking()
                .Where(x => x.IsActive == null || x.IsActive == 1)
                .GroupBy(x => x.ChemicalMasterId)
                .Select(x => new { ChemicalMasterId = x.Key, Qty = x.Sum(v => v.Qty) })
                .ToDictionaryAsync(x => x.ChemicalMasterId, x => x.Qty);

            var formulaTotals = formulaChemicals
                .GroupBy(x => x.FormulaMasterId)
                .ToDictionary(x => x.Key, x => x.Sum(v => v.Qty));
            var used = new Dictionary<int, double>();

            foreach (var mixture in mixtures)
            {
                if (!formulaTotals.TryGetValue(mixture.FormulaMasterId, out var formulaTotal) || formulaTotal <= 0)
                    continue;

                foreach (var chemical in formulaChemicals.Where(x => x.FormulaMasterId == mixture.FormulaMasterId))
                {
                    var consumed = (double)mixture.TotalMixture * chemical.Qty / formulaTotal;
                    used[chemical.ChemicalMasterId] = used.GetValueOrDefault(chemical.ChemicalMasterId) + consumed;
                }
            }

            var chemicalStock = chemicals.Select(chemical =>
            {
                var receipts = inward.Where(x => x.ChemicalMasterId == chemical.Id).ToList();
                var received = receipts.Sum(x => x.Qty);
                var usedQty = used.GetValueOrDefault(chemical.Id);
                var returned = returns.GetValueOrDefault(chemical.Id);
                return new ChemicalStockDto
                {
                    ChemicalMasterId = chemical.Id,
                    ChemicalName = chemical.Name,
                    Unit = string.Join(", ", receipts.Select(x => x.Unit).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct()),
                    Received = received,
                    Used = usedQty,
                    Returned = returned,
                    Balance = received + returned - usedQty
                };
            }).ToList();

            var mixtureStock = await db.MixtureForms.AsNoTracking()
                .GroupBy(x => new
                {
                    x.FormulaMasterId,
                    MixtureName = x.MixtureName ?? x.FormulaMaster!.MixtureName,
                    ProductName = x.FormulaMaster!.FinalProduct!.Final_Product
                })
                .Select(group => new RawMaterialStockDto
                {
                    MasterId = group.Key.FormulaMasterId,
                    Name = group.Key.ProductName + " - " + group.Key.MixtureName,
                    Unit = "KG",
                    Received = group.Sum(x => (double)x.TotalMixture),
                    Balance = group.Sum(x => (double)x.TotalMixture)
                })
                .OrderBy(x => x.Name)
                .ToListAsync();

            var fabricInwards = await db.FabricInward.AsNoTracking()
                .Where(x => x.IsActive == null || x.IsActive == 1)
                .Select(x => new
                {
                    x.Id,
                    x.FabricMasterId,
                    x.FGramageMasterId,
                    x.ColourMasterId,
                    Name = x.Fabric != null ? x.Fabric.Name : string.Empty,
                    Gramage = x.FGramage != null ? x.FGramage.GRM : string.Empty,
                    Colour = x.Colour != null ? x.Colour.Name : string.Empty,
                    x.QtyMTR
                })
                .ToListAsync();
            var rolling = await db.ClothRollingForms.AsNoTracking()
                .Where(x => x.IsActive == null || x.IsActive == 1)
                .Select(x => new { x.FabricInwardId, x.RollMtr, x.DefectMtr })
                .ToListAsync();
            var fabricReturns = await db.FabricStockReturns.AsNoTracking()
                .Where(x => x.IsActive == null || x.IsActive == 1)
                .Select(x => new { x.FabricInwardId, x.QtyMtr })
                .ToListAsync();

            var fabricStock = fabricInwards
                .GroupBy(x => new
                {
                    x.FabricMasterId,
                    x.FGramageMasterId,
                    x.ColourMasterId,
                    x.Name,
                    x.Gramage,
                    x.Colour
                })
                .Select(group =>
                {
                    var inwardIds = group.Select(x => x.Id).ToHashSet();
                    var relevantRolls = rolling.Where(x => x.FabricInwardId.HasValue && inwardIds.Contains(x.FabricInwardId.Value)).ToList();
                    var received = group.Sum(x => x.QtyMTR);
                    var actual = relevantRolls.Sum(x => (double)x.RollMtr);
                    var defective = relevantRolls.Sum(x => (double)x.DefectMtr);
                    var returned = fabricReturns.Where(x => inwardIds.Contains(x.FabricInwardId)).Sum(x => x.QtyMtr);
                    return new RawMaterialStockDto
                    {
                        MasterId = group.Key.FabricMasterId,
                        Name = group.Key.Name,
                        Unit = "MTR",
                        Gramage = group.Key.Gramage,
                        Colour = group.Key.Colour,
                        Received = received,
                        Actual = actual,
                        Defective = defective,
                        Variance = actual + defective - received,
                        Used = actual + defective,
                        Returned = returned,
                        Balance = received - actual - defective - returned
                    };
                }).OrderBy(x => x.Name).ToList();

            var pvcStock = await db.PVCInward.AsNoTracking()
                .Where(x => x.IsActive == null || x.IsActive == 1)
                .GroupBy(x => new { x.PVCMasterId, Name = x.PVC != null ? x.PVC.Name : string.Empty })
                .Select(x => new RawMaterialStockDto
                {
                    MasterId = x.Key.PVCMasterId,
                    Name = x.Key.Name,
                    Unit = "KG",
                    Received = x.Sum(v => v.Qty_kg),
                    Balance = x.Sum(v => v.Qty_kg)
                }).OrderBy(x => x.Name).ToListAsync();

            return Results.Ok(new StockManagementDto
            {
                Chemicals = chemicalStock,
                Mixtures = mixtureStock,
                Fabrics = fabricStock,
                PVC = pvcStock
            });
        });

        group.MapPost("/chemical-returns", async (CreateChemicalStockReturnDto dto, AppDbContext db) =>
        {
            if (dto.ChemicalMasterId <= 0 || !await db.Chemical.AnyAsync(x => x.Id == dto.ChemicalMasterId))
                return Results.BadRequest("Select a valid chemical.");
            if (dto.Qty <= 0)
                return Results.BadRequest("Return quantity must be greater than zero.");

            var entity = new ChemicalStockReturn
            {
                ChemicalMasterId = dto.ChemicalMasterId,
                Qty = dto.Qty,
                ReturnDate = dto.ReturnDate ?? DateTime.UtcNow,
                Remarks = dto.Remarks?.Trim(),
                IsActive = 1
            };
            db.ChemicalStockReturns.Add(entity);
            await db.SaveChangesAsync();
            return Results.Created($"/api/stock-management/chemical-returns/{entity.Id}", new { entity.Id });
        });

        group.MapPost("/fabric-returns", async (CreateFabricStockReturnDto dto, AppDbContext db) =>
        {
            var inward = await db.FabricInward.FirstOrDefaultAsync(x =>
                x.Id == dto.FabricInwardId && (x.IsActive == null || x.IsActive == 1));
            if (inward == null)
                return Results.BadRequest("Select a valid fabric inward batch.");
            if (dto.QtyMtr <= 0)
                return Results.BadRequest("Return MTR must be greater than zero.");

            var alreadyReturned = await db.FabricStockReturns
                .Where(x => x.FabricInwardId == inward.Id && (x.IsActive == null || x.IsActive == 1))
                .SumAsync(x => x.QtyMtr);
            if (alreadyReturned + dto.QtyMtr > inward.QtyMTR)
                return Results.BadRequest("Total returned MTR cannot exceed inward MTR.");

            var entity = new FabricStockReturn
            {
                FabricInwardId = inward.Id,
                QtyMtr = dto.QtyMtr,
                ReturnDate = dto.ReturnDate ?? DateTime.UtcNow,
                Remarks = dto.Remarks?.Trim(),
                IsActive = 1
            };
            db.FabricStockReturns.Add(entity);
            await db.SaveChangesAsync();
            return Results.Created($"/api/stock-management/fabric-returns/{entity.Id}", new { entity.Id });
        });
    }
}
