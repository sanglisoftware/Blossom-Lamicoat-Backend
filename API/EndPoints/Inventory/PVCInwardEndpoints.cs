using System.Text.RegularExpressions;
using Api.Application.DTOs;
using Api.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Api.API.EndPoints.Inventory
{
    public static partial class PVCInwardEndpoints
    {
        public static void MapPVCInwardEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/pvcinward").RequireAuthorization();

            // GET all PVCInward
            group.MapGet("/", async (HttpRequest req, IPVCInwardService service, ILoggerFactory loggerFactory) =>
            {
                try
                {
                    var query = RegexParseFilterSort.BindPagedQueryDto(req.Query);
                    var paged = await service.GetAllAsync(query);
                    return Results.Ok(paged);
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger("PVCInwardEndpoints");
                    logger.LogError(ex, "Failed to load PVC inward list");

                    return Results.Problem(
                        title: "Failed to load PVC inward list",
                        detail: ex.InnerException?.Message ?? ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError
                    );
                }
            }).RequireAuthorization();
            //Get all PVCInward for tabulator
            group.MapGet("/tabulator", GetPagedPVCInward).RequireAuthorization();

            // GET PVCInward by ID
            group.MapGet("/{id:int}", async (int id, IPVCInwardService service) =>
            {
                var pvcInward = await service.GetByIdAsync(id);
                return pvcInward is null ? Results.NotFound() : Results.Ok(pvcInward);
            });

            // POST create new PVCInward
            
               group.MapPost("/", async (
                    HttpContext httpContext,
                    [FromForm] int supplierMasterId,
                    [FromForm] int pvcMasterId,
                    [FromForm] string new_RollNo,
                    [FromForm] double batchNo,
                    [FromForm] double qty_kg,
                    [FromForm] double qty_Mtr,
                    [FromForm] string comments,
                    [FromForm] int? gramageMasterId,
                    [FromForm] string? gramageName,
                    [FromForm] int? widthMasterId,
                    [FromForm] string? widthName,
                    [FromForm] int? colourMasterId,
                    [FromForm] string? colourName,
                    [FromForm] DateTime? billDate,
                    [FromForm] DateTime? receivedDate,
                    [FromForm] short? isActive,
                    IFormFile? attachedFile,
                    IPVCInwardService service) =>
            {
                try
                {
                    var dto = new PVCInwardDto
                    {
                        SupplierMasterId = supplierMasterId,
                        PVCMasterId = pvcMasterId,
                        New_RollNo = new_RollNo,
                        BatchNo = batchNo,
                        Qty_kg = qty_kg,
                        Qty_Mtr = qty_Mtr,
                        Comments = comments,
                        GramageMasterId = gramageMasterId,
                        GramageName = gramageName,
                        WidthMasterId = widthMasterId,
                        WidthName = widthName,
                        ColourMasterId = colourMasterId,
                        ColourName = colourName,
                        BillDate = billDate,
                        ReceivedDate = receivedDate,
                        IsActive = isActive,
                        AttachedFile = await SaveAttachedFileAsync(httpContext, attachedFile),
                    };

                    var created = await service.CreateAsync(dto);
                    return Results.Created($"/api/pvcinward/{created.Id}", created);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            });

            // PUT update PVCInward
            group.MapPut("/{id:int}", async (
                int id,
                HttpContext httpContext,
                [FromForm] int supplierMasterId,
                [FromForm] int pvcMasterId,
                [FromForm] string new_RollNo,
                [FromForm] double batchNo,
                [FromForm] double qty_kg,
                [FromForm] double qty_Mtr,
                [FromForm] string comments,
                [FromForm] int? gramageMasterId,
                [FromForm] string? gramageName,
                [FromForm] int? widthMasterId,
                [FromForm] string? widthName,
                [FromForm] int? colourMasterId,
                [FromForm] string? colourName,
                [FromForm] DateTime? billDate,
                [FromForm] DateTime? receivedDate,
                [FromForm] short? isActive,
                [FromForm] string? existingAttachedFile,
                IFormFile? attachedFile,
                IPVCInwardService service) =>
            {
                try
                {
                    var dto = new PVCInwardDto
                    {
                        Id = id,
                        SupplierMasterId = supplierMasterId,
                        PVCMasterId = pvcMasterId,
                        New_RollNo = new_RollNo,
                        BatchNo = batchNo,
                        Qty_kg = qty_kg,
                        Qty_Mtr = qty_Mtr,
                        Comments = comments,
                        GramageMasterId = gramageMasterId,
                        GramageName = gramageName,
                        WidthMasterId = widthMasterId,
                        WidthName = widthName,
                        ColourMasterId = colourMasterId,
                        ColourName = colourName,
                        BillDate = billDate,
                        ReceivedDate = receivedDate,
                        IsActive = isActive,
                        AttachedFile = await SaveAttachedFileAsync(httpContext, attachedFile) ?? existingAttachedFile,
                    };

                    var updated = await service.UpdateAsync(id, dto);
                    return updated is null ? Results.NotFound() : Results.Ok(updated);
                }
                catch (Exception ex)
                {
                    return Results.Problem("Username Alredy Exist" + ex.Message);
                }
            });

            // DELETE PVCInward
            group.MapDelete("/{id:int}", async (int id, IPVCInwardService service) =>
            {
                return await service.DeleteAsync(id) ? Results.NoContent() : Results.NotFound();
            });

            group.MapPatch("/{id}/status", async (int id, [FromBody] short IsActive, IPVCInwardService service) =>
            {
                var updatedPVCInward = await service.UpdateStatusAsync(id, IsActive);
                return updatedPVCInward is null
                    ? Results.Problem("Failed to update status")
                    : Results.Ok(updatedPVCInward);
            }).RequireAuthorization();
        }

        private static async Task<IResult> GetPagedPVCInward(HttpRequest req, IPVCInwardService service, ILoggerFactory loggerFactory)
        {
            try
            {
                var query = BindPagedQueryDto(req.Query);
                var paged = await service.GetAllAsync(query);
                return Results.Ok(paged);
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger("PVCInwardEndpoints");
                logger.LogError(ex, "Failed to load PVC inward tabulator list");

                return Results.Problem(
                    title: "Failed to load PVC inward tabulator list",
                    detail: ex.InnerException?.Message ?? ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
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

        private static async Task<string?> SaveAttachedFileAsync(HttpContext httpContext, IFormFile? attachedFile)
        {
            if (attachedFile == null || attachedFile.Length == 0)
            {
                return null;
            }

            var uploadsFolder = Path.Combine(
                httpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().WebRootPath ?? Path.Combine(AppContext.BaseDirectory, "wwwroot"),
                "uploads",
                "pvcinward"
            );

            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid():N}_{Path.GetFileName(attachedFile.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            await using var stream = File.Create(filePath);
            await attachedFile.CopyToAsync(stream);

            return $"/uploads/pvcinward/{fileName}";
        }
    }
}
