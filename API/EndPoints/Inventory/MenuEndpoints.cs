using Api.Application.DTOs;
using Api.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.API.EndPoints.Inventory
{
    public static class MenuEndpoints
    {
        public static void MapMenuEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/menu").WithTags("Menu").RequireAuthorization();

            group.MapGet("/", async ([FromServices] IMenuService menuService, HttpContext context) =>
            {
                // var roleIdClaim = context.User.FindFirst("roleId");
                // if (roleIdClaim == null || !int.TryParse(roleIdClaim.Value, out int roleId))
                // {
                //     return Results.Unauthorized();
                // }

                var menu = await menuService.GetMenuForRoleAsync(1);
                return Results.Ok(menu);
            }).WithName("GetUserMenu")
            .Produces<List<MenuDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized).WithOpenApi();

            group.MapPost("/permissions", async ([FromBody] RoleMenuPermissionRequest request, [FromServices] IMenuService menuService) =>
                    {
                        if (request?.Permissions == null) return Results.BadRequest("Invalid request payload");

                        try
                        {
                            var success = await menuService.SavePermissionsForRoleAsync(request);
                            return success ? Results.Ok("Permissions saved successfully") : Results.BadRequest("Invalid menu IDs detected");
                        }
                        catch (Exception ex) { return Results.Problem(ex.Message); }
                    }
                )
                .WithName("SaveRolePermissions")
                .Produces<string>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status500InternalServerError)
                .WithOpenApi();

            // In MenuEndpoints.cs
            group.MapGet("/all-with-selection/{roleId}", async (int roleId, [FromServices] IMenuService menuService) =>
            {
                var menu = await menuService.GetAllMenusWithSelectionAsync(roleId);
                return Results.Ok(menu);
            })
            .WithName("GetAllMenusWithSelection")
            .Produces<List<MenuSelectionDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithOpenApi();
        }
    }
}
