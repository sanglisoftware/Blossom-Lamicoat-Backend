using Api.Application.DTOs;
using Api.Application.Interfaces;

namespace Api.API.EndPoints.Inventory;

public static class MixtureFormEndpoints
{
    public static void MapMixtureFormEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/mixtureform").RequireAuthorization();

        group.MapGet("", async (HttpRequest req, IMixtureFormService service) =>
        {
            var query = RegexParseFilterSort.BindPagedQueryDto(req.Query);
            var paged = await service.GetAllAsync(query);
            return Results.Ok(paged);
        });

        group.MapGet("/{id:int}", async (int id, IMixtureFormService service) =>
        {
            var mixtureForm = await service.GetByIdAsync(id);
            return mixtureForm is null ? Results.NotFound() : Results.Ok(mixtureForm);
        });

        group.MapPost("", async (MixtureFormDto dto, IMixtureFormService service) =>
        {
            try
            {
                var created = await service.CreateAsync(dto);
                return Results.Created($"/api/mixtureform/{created.Id}", created);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });
    }
}
