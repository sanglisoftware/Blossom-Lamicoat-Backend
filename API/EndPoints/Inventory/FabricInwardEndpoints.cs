using System.Text.RegularExpressions;
using Api.Application.DTOs;
using Api.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.API.EndPoints.Inventory
{
    public static partial class FabricInwardEndpoints
    {
        public static void MapFabricInwardEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/fabricinward").RequireAuthorization();

            // GET all FabricInward
            group.MapGet("/", async (HttpRequest req, IFabricInwardService service) =>
            {
                var query = RegexParseFilterSort.BindPagedQueryDto(req.Query);
                var paged = await service.GetAllAsync(query);
                return Results.Ok(paged);
            }).RequireAuthorization();
            //Get all FabricInward for tabulator
            group.MapGet("/tabulator", GetPagedFabricInward).RequireAuthorization();

            // GET FabricInward by ID
            group.MapGet("/{id:int}", async (int id, IFabricInwardService service) =>
            {
                var fabricInward = await service.GetByIdAsync(id);
                return fabricInward is null ? Results.NotFound() : Results.Ok(fabricInward);
            });

            // POST create new FabricInward
              // POST create new colour
            group.MapPost("/", async (HttpContext httpContext, IFabricInwardService service) =>
            {
                try
                {
                    var dto = await BuildFabricInwardDtoAsync(httpContext);
                    var created = await service.CreateAsync(dto);
                    return Results.Created($"/api/fabricinward/{created.Id}", created);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            }).DisableAntiforgery();

            // PUT update FabricInward
            group.MapPut("/{id:int}", async (int id, HttpContext httpContext, IFabricInwardService service) =>
            {
                try
                {
                    var dto = await BuildFabricInwardDtoAsync(httpContext, id);
                    var updated = await service.UpdateAsync(id, dto);
                    return updated is null ? Results.NotFound() : Results.Ok(updated);
                }
                catch (Exception ex)
                {
                    return Results.Problem("Username Alredy Exist" + ex.Message);
                }
            }).DisableAntiforgery();

            // DELETE FabricInward
            group.MapDelete("/{id:int}", async (int id, IFabricInwardService service) =>
            {
                return await service.DeleteAsync(id) ? Results.NoContent() : Results.NotFound();
            });

            group.MapPatch("/{id}/status", async (int id, [FromBody] short IsActive, IFabricInwardService service) =>
            {
                var updatedFabricInward = await service.UpdateStatusAsync(id, IsActive);
                return updatedFabricInward is null
                    ? Results.Problem("Failed to update status")
                    : Results.Ok(updatedFabricInward);
            }).RequireAuthorization();
        }

        private static async Task<IResult> GetPagedFabricInward(HttpRequest req, IFabricInwardService service)
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

        private static async Task<FabricInwardDto> BuildFabricInwardDtoAsync(HttpContext httpContext, int? id = null)
        {
            if (!httpContext.Request.HasFormContentType)
            {
                var dto = await httpContext.Request.ReadFromJsonAsync<FabricInwardDto>();
                if (dto == null)
                {
                    throw new BadHttpRequestException("Invalid request body.");
                }

                if (id.HasValue)
                {
                    dto.Id = id.Value;
                }

                return dto;
            }

            var form = await httpContext.Request.ReadFormAsync();
            var dtoFromForm = new FabricInwardDto
            {
                Id = id ?? ParseRequiredInt(form, "id"),
                SupplierMasterId = ParseRequiredInt(form, "supplierMasterId"),
                FabricMasterId = ParseRequiredInt(form, "fabricMasterId"),
                FGramageMasterId = ParseNullableInt(form, "fGramageMasterId"),
                ColourMasterId = ParseNullableInt(form, "colourMasterId"),
                BatchNo = ParseRequiredDouble(form, "batchNo"),
                QtyMTR = ParseRequiredDouble(form, "qtyMTR"),
                Comments = form["comments"].ToString(),
                IsActive = ParseNullableShort(form, "isActive"),
                AttachedFile = await SaveAttachedFileAsync(httpContext, form.Files["attachedFile"])
                    ?? form["existingAttachedFile"].ToString(),
            };

            return dtoFromForm;
        }

        private static int ParseRequiredInt(IFormCollection form, string key)
        {
            var rawValue = form[key].ToString();
            if (!int.TryParse(rawValue, out var value))
            {
                throw new BadHttpRequestException($"{key} is required.");
            }

            return value;
        }

        private static int? ParseNullableInt(IFormCollection form, string key)
        {
            var rawValue = form[key].ToString();
            return int.TryParse(rawValue, out var value) ? value : null;
        }

        private static double ParseRequiredDouble(IFormCollection form, string key)
        {
            var rawValue = form[key].ToString();
            if (!double.TryParse(rawValue, out var value))
            {
                throw new BadHttpRequestException($"{key} is required.");
            }

            return value;
        }

        private static short? ParseNullableShort(IFormCollection form, string key)
        {
            var rawValue = form[key].ToString();
            return short.TryParse(rawValue, out var value) ? value : null;
        }

        private static async Task<string?> SaveAttachedFileAsync(HttpContext httpContext, IFormFile? attachedFile)
        {
            if (attachedFile == null || attachedFile.Length == 0)
            {
                return null;
            }

            var uploadsFolder = Path.Combine(
                httpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().WebRootPath,
                "uploads",
                "fabricinward"
            );

            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid():N}_{Path.GetFileName(attachedFile.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await attachedFile.CopyToAsync(stream);

            return $"/uploads/fabricinward/{fileName}";
        }
    }
}
