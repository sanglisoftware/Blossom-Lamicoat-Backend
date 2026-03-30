using Api.Application.DTOs;
using Api.Application.Interfaces;

namespace Api.API.EndPoints.Inventory;

public static class LaminationFormEndpoints
{
    public static void MapLaminationFormEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/laminationform").RequireAuthorization();

        group.MapGet("", async (HttpRequest req, ILaminationFormService service) =>
        {
            var query = RegexParseFilterSort.BindPagedQueryDto(req.Query);
            var paged = await service.GetAllAsync(query);
            return Results.Ok(paged);
        });

        group.MapGet("/{id:int}", async (int id, ILaminationFormService service) =>
        {
            var laminationForm = await service.GetByIdAsync(id);
            return laminationForm is null ? Results.NotFound() : Results.Ok(laminationForm);
        });

        group.MapPost("", async (LaminationFormDto dto, ILaminationFormService service) =>
        {
            try
            {
                var created = await service.CreateAsync(dto);
                return Results.Created($"/api/laminationform/{created.Id}", created);
            }
            catch (Exception ex)
            {
                var detail = ex.InnerException?.Message ?? ex.Message;
                return Results.Problem(detail);
            }
        });
    }
}
