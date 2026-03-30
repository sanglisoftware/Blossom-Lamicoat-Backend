using Api.Application.DTOs;
using Api.Application.Interfaces;

namespace Api.API.EndPoints.Inventory;

public static class InspectionFormEndpoints
{
    public static void MapInspectionFormEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/inspectionform").RequireAuthorization();

        group.MapGet("", async (HttpRequest req, IInspectionFormService service) =>
        {
            var query = RegexParseFilterSort.BindPagedQueryDto(req.Query);
            var paged = await service.GetAllAsync(query);
            return Results.Ok(paged);
        });

        group.MapGet("/{id:int}", async (int id, IInspectionFormService service) =>
        {
            var inspectionForm = await service.GetByIdAsync(id);
            return inspectionForm is null ? Results.NotFound() : Results.Ok(inspectionForm);
        });

        group.MapPost("", async (InspectionFormDto dto, IInspectionFormService service) =>
        {
            try
            {
                var created = await service.CreateAsync(dto);
                return Results.Created($"/api/inspectionform/{created.Id}", created);
            }
            catch (Exception ex)
            {
                var detail = ex.InnerException?.Message ?? ex.Message;
                return Results.Problem(detail);
            }
        });
    }
}
