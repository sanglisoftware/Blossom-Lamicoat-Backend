using Api.API.EndPoints.Inventory;
using Api.Application.DTOs;
using Api.Application.Interfaces;

namespace Api.API.EndPoints.Inventory;

public static class SalaryEndpoints
{
    public static void MapSalaryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/salary").RequireAuthorization();

        group.MapGet("", async (HttpRequest req, ISalaryService service) =>
        {
            var query = RegexParseFilterSort.BindPagedQueryDto(req.Query);
            var paged = await service.GetAllAsync(query);
            return Results.Ok(paged);
        });

        group.MapGet("/{id:int}", async (int id, ISalaryService service) =>
        {
            var salary = await service.GetByIdAsync(id);
            return salary is null ? Results.NotFound() : Results.Ok(salary);
        });

        group.MapPost("", async (SalaryDto dto, ISalaryService service) =>
        {
            try
            {
                var created = await service.CreateAsync(dto);
                return Results.Created($"/api/salary/{created.Id}", created);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });
    }
}
