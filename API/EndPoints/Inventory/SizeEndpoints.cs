using Api.Application.Interfaces;
using Api.Application.DTOs;

namespace Api.API.EndPoints.Inventory
{
    public static class SizeEndpoints
    {
        public static void MapSizeEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/sizes");

            group.MapGet("/", async (ISizeService service) => Results.Ok(await service.GetAllAsync())).RequireAuthorization();

            group.MapGet("/{id:int}", async (int id, ISizeService service) =>
            {
                var size = await service.GetByIdAsync(id);
                return size is null ? Results.NotFound() : Results.Ok(size);
            }).RequireAuthorization();

            group.MapPost("/", async (SizeDto dto, ISizeService service) =>
            {
                var created = await service.CreateAsync(dto);
                return Results.Created($"/api/sizes/{created.Id}", created);
            }).RequireAuthorization();

            group.MapPut("/{id:int}", async (int id, SizeDto dto, ISizeService service) =>
            {
                var updated = await service.UpdateAsync(id, dto);
                return updated is null ? Results.NotFound() : Results.Ok(updated);
            }).RequireAuthorization();

            group.MapDelete("/{id:int}", async (int id, ISizeService service) => { return await service.DeleteAsync(id) ? Results.NoContent() : Results.NotFound(); }).RequireAuthorization();
        }
    }
}