using Api.Application.DTOs;
using Api.Application.Interfaces;

namespace Api.API.EndPoints.Inventory
{
    public static class GalleryFilterEndpoints
    {
        public static void MapGalleryFilterEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/GalleryFilters");

            group.MapGet("/tabulator", GetPagedFilters).RequireAuthorization();

            group.MapGet("/", async (IGalleryFilterService service) => Results.Ok(await service.GetAllAsync())).RequireAuthorization();

            group.MapGet("/{id:int}", async (int id, IGalleryFilterService service) =>
            {
                var Filter = await service.GetByIdAsync(id);
                return Filter is null ? Results.NotFound() : Results.Ok(Filter);
            }).RequireAuthorization();

            group.MapPost("/", async (GalleryFilterDto dto, IGalleryFilterService service) =>
            {
                var created = await service.CreateAsync(dto);
                return Results.Created($"/api/GalleryFilters/{created.Id}", created);
            }).RequireAuthorization();

            group.MapPut("/{id:int}", async (int id, GalleryFilterDto dto, IGalleryFilterService service) =>
            {
                var updated = await service.UpdateAsync(id, dto);
                return updated is null ? Results.NotFound() : Results.Ok(updated);
            }).RequireAuthorization();

            group.MapDelete("/{id:int}", async (int id, IGalleryFilterService service) => { return await service.DeleteAsync(id) ? Results.NoContent() : Results.NotFound(); }).RequireAuthorization();
        }

        private static async Task<IResult> GetPagedFilters(HttpRequest req, IGalleryFilterService service)
        {
            var query = RegexParseFilterSort.BindPagedQueryDto(req.Query);
            var paged = await service.GetAllAsync(query);
            return Results.Ok(paged);
        }
    }
}