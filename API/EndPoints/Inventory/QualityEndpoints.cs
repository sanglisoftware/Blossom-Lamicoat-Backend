using System.Text.RegularExpressions;
using Api.Application.DTOs;
using Api.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.API.EndPoints.Inventory
{
    public static partial class QualityEndpoints
    {
        public static void MapQualityEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/quality").RequireAuthorization();

            // GET all Fabric productList
            group.MapGet("/", async (HttpRequest req, IQualityService service) =>
            {
                var query = RegexParseFilterSort.BindPagedQueryDto(req.Query);
                var paged = await service.GetAllAsync(query);
                return Results.Ok(paged);
            }).RequireAuthorization();
            //Get all Fabric productList for tabulator
            group.MapGet("/tabulator", GetPagedQuality).RequireAuthorization();

            // GET Fabric productList by ID
            group.MapGet("/{id:int}", async (int id, IQualityService service) =>
            {
                var quality = await service.GetByIdAsync(id);
                return quality is null ? Results.NotFound() : Results.Ok(quality);
            });

            // POST create new PVCproductList
               group.MapPost("/", async (QualityDto dto, IQualityService service) =>
            {
                try
                {
                    var created = await service.CreateAsync(dto);
                    return Results.Created($"/api/quality/{created.Id}", created);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            });

            // PUT update Fabric productList
            group.MapPut("/{id:int}", async (int id, QualityDto dto, IQualityService service) =>
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

            // DELETE Fabric productList
            group.MapDelete("/{id:int}", async (int id, IQualityService service) =>
            {
                return await service.DeleteAsync(id) ? Results.NoContent() : Results.NotFound();
            });

            group.MapPatch("/{id}/status", async (int id, [FromBody] short IsActive, IQualityService service) =>
            {
                var updatedQuality = await service.UpdateStatusAsync(id, IsActive);
                return updatedQuality is null
                    ? Results.Problem("Failed to update status")
                    : Results.Ok(updatedQuality);
            }).RequireAuthorization();
        }

        private static async Task<IResult> GetPagedQuality(HttpRequest req, IQualityService service)
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
