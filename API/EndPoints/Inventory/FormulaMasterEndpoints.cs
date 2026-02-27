using System.Text.RegularExpressions;
using Api.Application.DTOs;
using Api.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.API.EndPoints.Inventory
{
    public static partial class FormulaMasterEndpoints
    {
        public static void MapFormulaMasterEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/formulamaster").RequireAuthorization();

            // GET all FormulaMaster
            group.MapGet("/", async (HttpRequest req, IFormulaMasterService service) =>
            {
                var query = RegexParseFilterSort.BindPagedQueryDto(req.Query);
                var paged = await service.GetAllAsync(query);
                return Results.Ok(paged);
            }).RequireAuthorization();
            //Get all FormulaMaster for tabulator
            group.MapGet("/tabulator", GetPagedFormulaMaster).RequireAuthorization();

            // GET FormulaMaster by ID
            group.MapGet("/{id:int}", async (int id, IFormulaMasterService service) =>
            {
                var formulamaster = await service.GetByIdAsync(id);
                return formulamaster is null ? Results.NotFound() : Results.Ok(formulamaster);
            });

            // POST create new FormulaMaster
               group.MapPost("/", async (FormulaMasterDto dto, IFormulaMasterService service) =>
            {
                try
                {
                    var created = await service.CreateAsync(dto);
                    return Results.Created($"/api/formulamaster/{created.Id}", created);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            });

            // PUT update FormulaMaster
            group.MapPut("/{id:int}", async (int id, FormulaMasterDto dto, IFormulaMasterService service) =>
            {
                try
                {
                    var updated = await service.UpdateAsync(id, dto);
                    return updated is null ? Results.NotFound() : Results.Ok(updated);
                }
                catch (Exception ex)
                {
                    return Results.Problem("Username Alredy Exist" + ex.Message);
                }
            });

            // DELETE FormulaMaster
            group.MapDelete("/{id:int}", async (int id, IFormulaMasterService service) =>
            {
                return await service.DeleteAsync(id) ? Results.NoContent() : Results.NotFound();
            });

            group.MapPatch("/{id}/status", async (int id, [FromBody] short IsActive, IFormulaMasterService service) =>
            {
                var updatedFormulaMaster = await service.UpdateStatusAsync(id, IsActive);
                return updatedFormulaMaster is null
                    ? Results.Problem("Failed to update status")
                    : Results.Ok(updatedFormulaMaster);
            }).RequireAuthorization();
        }

        private static async Task<IResult> GetPagedFormulaMaster(HttpRequest req, IFormulaMasterService service)
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
