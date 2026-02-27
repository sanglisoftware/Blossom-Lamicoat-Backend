using System.Text.RegularExpressions;
using Api.Application.DTOs;
using Api.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.API.EndPoints.Inventory
{
    public static partial class FormulaChemicalTransactionEndpoints
    {
        public static void MapFormulaChemicalTransactionEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/formulachemicaltransaction").RequireAuthorization();

            // GET all FormulaMaster
            group.MapGet("/", async (HttpRequest req, IFormulaChemicalTransactionService service) =>
            {
                var query = RegexParseFilterSort.BindPagedQueryDto(req.Query);
                var paged = await service.GetAllAsync(query);
                return Results.Ok(paged);
            }).RequireAuthorization();
            //Get all FormulaMaster for tabulator
            group.MapGet("/tabulator", GetPagedFormulaChemicalTransaction).RequireAuthorization();

            // GET FormulaMaster by ID
            group.MapGet("/{id:int}", async (int id, IFormulaChemicalTransactionService service) =>
            {
                var formulachemicaltransaction = await service.GetByIdAsync(id);
                return formulachemicaltransaction is null ? Results.NotFound() : Results.Ok(formulachemicaltransaction);
            });

            // POST create new FormulaMaster
            group.MapPost("/", async (FormulaChemicalTransactionDto dto, IFormulaChemicalTransactionService service) =>
         {
             try
             {
                 var created = await service.CreateAsync(dto);
                 return Results.Created($"/api/FormulaChemicalTransaction/{created.Id}", created);
             }
             catch (Exception ex)
             {
                 return Results.Problem(ex.Message);
             }
         });

            // PUT update FormulaMaster
            // group.MapPut("/{id:int}", async (int id, FormulaChemicalTransactionDto dto, IFormulaChemicalTransactionService service) =>
            // {
            //     try
            //     {
            //         var updated = await service.UpdateAsync(id, dto);
            //         return updated is null ? Results.NotFound() : Results.Ok(updated);
            //     }
            //     catch (Exception ex)
            //     {
            //         return Results.Problem("Username Alredy Exist" + ex.Message);
            //     }
            // });

            // PUT update FormulaMaster (one-to-many)
group.MapPut("/", async (FormulaDetailsDto dto, IFormulaChemicalTransactionService service) =>
{
    try
    {
        var updated = await service.UpdateFormulaAsync(dto);
        return updated is null ? Results.NotFound() : Results.Ok(updated);
    }
    catch (Exception ex)
    {
        return Results.Problem("Error updating formula: " + ex.Message);
    }
});


            // DELETE FormulaMaster
            group.MapDelete("/{id:int}", async (int id, IFormulaChemicalTransactionService service) =>
            {
                return await service.DeleteAsync(id) ? Results.NoContent() : Results.NotFound();
            });

        }

        private static async Task<IResult> GetPagedFormulaChemicalTransaction(HttpRequest req, IFormulaChemicalTransactionService service)
        {
            var query = BindPagedQueryDto(req.Query);
            var paged = await service.GetAllAsync(query);
            return Results.Ok(paged);
        }
        private static PagedQueryDto BindPagedQueryDto(IQueryCollection q)
        {
            var dto = new PagedQueryDto();
            var filters = new Dictionary<int, FilterDto>();
            var sorts = new Dictionary<int, SortDto>();

            // parse page & size
            if (q.TryGetValue("page", out var pg) && int.TryParse(pg, out var pi))
                dto.page = pi;
            if (q.TryGetValue("size", out var sz) && int.TryParse(sz, out var si))
                dto.size = si;

            // regex for filter keys
            var rf = MyRegex1();
            foreach (var kv in q)
            {
                var m = rf.Match(kv.Key);
                if (!m.Success)
                    continue;
                var idx = int.Parse(m.Groups[1].Value);
                var prop = m.Groups[2].Value;
                if (!filters.TryGetValue(idx, out var fd))
                    filters[idx] = fd = new();
                switch (prop)
                {
                    case "field":
                        fd.Field = kv.Value.ToString() ?? string.Empty;
                        break;
                    case "type":
                        fd.Type = kv.Value.ToString() ?? string.Empty;
                        break;
                    case "value":
                        fd.Value = kv.Value.ToString() ?? string.Empty;
                        break;
                }
            }
            dto.filter = filters.OrderBy(x => x.Key).Select(x => x.Value).ToList();

            // regex for sort keys
            var rs = MyRegex();
            foreach (var kv in q)
            {
                var m = rs.Match(kv.Key);
                if (!m.Success)
                    continue;
                var idx = int.Parse(m.Groups[1].Value);
                var prop = m.Groups[2].Value;
                if (!sorts.TryGetValue(idx, out var sd))
                    sorts[idx] = sd = new();
                switch (prop)
                {
                    case "field":
                        sd.Field = kv.Value.ToString() ?? string.Empty;
                        break;
                    case "dir":
                        sd.Dir = kv.Value.ToString() ?? string.Empty;
                        break;
                }
            }
            dto.sort = sorts.OrderBy(x => x.Key).Select(x => x.Value).ToList();

            return dto;
        }

        [GeneratedRegex(@"^sort\[(\d+)\]\[(field|dir)\]$")]
        private static partial Regex MyRegex();
        [GeneratedRegex(@"^filter\[(\d+)\]\[(field|type|value)\]$")]
        private static partial Regex MyRegex1();
    }
}
// using System.Text.RegularExpressions;
// using Api.Application.DTOs;
// using Api.Application.Interfaces;
// using Microsoft.AspNetCore.Mvc;

// namespace Api.API.EndPoints.Inventory
// {
//     public static partial class FormulaChemicalTransactionEndpoints
//     {
//         public static void MapFormulaChemicalTransactionEndpoints(this IEndpointRouteBuilder app)
//         {
//             var group = app.MapGroup("/api/formulachemicaltransaction").RequireAuthorization();

//             // GET Formula details by formulaMasterId
//             group.MapGet("/details/{formulaMasterId:int}", async (int formulaMasterId, IFormulaChemicalTransactionService service) =>
//             {
//                 var details = await service.GetFormulaDetailsAsync(formulaMasterId);
//                 return details is null ? Results.NotFound() : Results.Ok(details);
//             }).RequireAuthorization();

//             // GET FormulaMaster by ID (single transaction)
//             group.MapGet("/{id:int}", async (int id, IFormulaChemicalTransactionService service) =>
//             {
//                 var formulachemicaltransaction = await service.GetByIdAsync(id);
//                 return formulachemicaltransaction is null ? Results.NotFound() : Results.Ok(formulachemicaltransaction);
//             });

//             // POST create new FormulaMaster transaction
//             group.MapPost("/", async (FormulaChemicalTransactionDto dto, IFormulaChemicalTransactionService service) =>
//             {
//                 try
//                 {
//                     var created = await service.CreateAsync(dto);
//                     return Results.Created($"/api/FormulaChemicalTransaction/{created.Id}", created);
//                 }
//                 catch (Exception ex)
//                 {
//                     return Results.Problem(ex.Message);
//                 }
//             });

//             // PUT update FormulaMaster (one-to-many)
//             group.MapPut("/", async (FormulaDetailsDto dto, IFormulaChemicalTransactionService service) =>
//             {
//                 try
//                 {
//                     var updated = await service.UpdateFormulaAsync(dto);
//                     return updated is null ? Results.NotFound() : Results.Ok(updated);
//                 }
//                 catch (Exception ex)
//                 {
//                     return Results.Problem("Error updating formula: " + ex.Message);
//                 }
//             });

//             // DELETE FormulaMaster transaction
//             group.MapDelete("/{id:int}", async (int id, IFormulaChemicalTransactionService service) =>
//             {
//                 return await service.DeleteAsync(id) ? Results.NoContent() : Results.NotFound();
//             });
//         }
//     }
// }
