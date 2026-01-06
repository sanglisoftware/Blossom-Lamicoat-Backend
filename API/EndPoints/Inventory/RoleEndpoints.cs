using Api.Application.DTOs;
using Api.Application.Interfaces;

namespace Api.API.EndPoints.Inventory
{
    public static class RoleEndpoints
    {
        public static void MapRoleEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/Roles");

            group.MapGet("/tabulator", GetPagedCategories).RequireAuthorization();

            group.MapGet("/", async (IRoleService service) => Results.Ok(await service.GetAllAsync())).RequireAuthorization();

            group.MapGet("/{id:int}", async (int id, IRoleService service) =>
            {
                var Role = await service.GetByIdAsync(id);
                return Role is null ? Results.NotFound() : Results.Ok(Role);
            }).RequireAuthorization();

            group.MapPost("/", async (RoleDto dto, IRoleService service) =>
            {
                var created = await service.CreateAsync(dto);
                return Results.Created($"/api/Roles/{created.Id}", created);
            }).RequireAuthorization();

            group.MapPut("/{id:int}", async (int id, RoleDto dto, IRoleService service) =>
            {
                var updated = await service.UpdateAsync(id, dto);
                return updated is null ? Results.NotFound() : Results.Ok(updated);
            }).RequireAuthorization();

            group.MapDelete("/{id:int}", async (int id, IRoleService service) => { return await service.DeleteAsync(id) ? Results.NoContent() : Results.NotFound(); }).RequireAuthorization();
        }

        private static async Task<IResult> GetPagedCategories(HttpRequest req, IRoleService service)
        {
            var query = RegexParseFilterSort.BindPagedQueryDto(req.Query);
            var paged = await service.GetAllAsync(query);
            return Results.Ok(paged);
        }
    }
}