using Api.Application.DTOs;
using Api.Application.Interfaces;

namespace Api.API.EndPoints.Inventory;

public static class ClothRollingFormEndpoints
{
    public static void MapClothRollingFormEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/clothrollingform").RequireAuthorization();

        group.MapGet("", async (HttpRequest req, IClothRollingFormService service) =>
        {
            var query = RegexParseFilterSort.BindPagedQueryDto(req.Query);
            var paged = await service.GetAllAsync(query);
            return Results.Ok(paged);
        });

        group.MapGet("/{id:int}", async (int id, IClothRollingFormService service) =>
        {
            var clothRollingForm = await service.GetByIdAsync(id);
            return clothRollingForm is null ? Results.NotFound() : Results.Ok(clothRollingForm);
        });

        group.MapPost("", async (ClothRollingFormDto dto, IClothRollingFormService service) =>
        {
            try
            {
                var created = await service.CreateAsync(dto);
                return Results.Created($"/api/clothrollingform/{created.Id}", created);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });
    }
}
