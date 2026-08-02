using Api.Application.DTOs;
using Api.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.API.EndPoints.Inventory
{
    public static class UnitOfMeasurementEndpoints
    {
        public static void MapUnitOfMeasurementEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/unitofmeasurement").RequireAuthorization();

            group.MapGet("/", async (HttpRequest req, IUnitOfMeasurementService service) =>
            {
                var query = RegexParseFilterSort.BindPagedQueryDto(req.Query);
                var paged = await service.GetAllAsync(query);
                return Results.Ok(paged);
            });

            group.MapGet("/tabulator", async (HttpRequest req, IUnitOfMeasurementService service) =>
            {
                var query = RegexParseFilterSort.BindPagedQueryDto(req.Query);
                var paged = await service.GetAllAsync(query);
                return Results.Ok(paged);
            });

            group.MapGet("/{id:int}", async (int id, IUnitOfMeasurementService service) =>
            {
                var unit = await service.GetByIdAsync(id);
                return unit is null ? Results.NotFound() : Results.Ok(unit);
            });

            group.MapPost("/", async (UnitOfMeasurementDto dto, IUnitOfMeasurementService service) =>
            {
                try
                {
                    var created = await service.CreateAsync(dto);
                    return Results.Created($"/api/unitofmeasurement/{created.Id}", created);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            });

            group.MapPut("/{id:int}", async (int id, UnitOfMeasurementDto dto, IUnitOfMeasurementService service) =>
            {
                try
                {
                    var updated = await service.UpdateAsync(id, dto);
                    return updated is null ? Results.NotFound() : Results.Ok(updated);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            });

            group.MapDelete("/{id:int}", async (int id, IUnitOfMeasurementService service) =>
            {
                try
                {
                    return await service.DeleteAsync(id) ? Results.NoContent() : Results.NotFound();
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            });

            group.MapPatch("/{id:int}/status", async (int id, [FromBody] short isActive, IUnitOfMeasurementService service) =>
            {
                var updated = await service.UpdateStatusAsync(id, isActive);
                return updated is null ? Results.NotFound() : Results.Ok(updated);
            });
        }
    }
}
