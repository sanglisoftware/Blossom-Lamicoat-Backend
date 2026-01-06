using Api.Application.DTOs;
using Api.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.API.EndPoints.Inventory
{
    public static class ShopEndpoints
    {
        public static void MapShopEndpoints(this IEndpointRouteBuilder app)
        {
            var shops = app.MapGroup("/api/inventory/shops");

            // POST - create shop
            shops.MapPost("/", async ([FromBody] ShopDto dto, IShopService service) =>
            {
                var created = await service.CreateShopAsync(dto);
                return Results.Created($"/api/inventory/shops/{created.Id}", created);
            }).RequireAuthorization();

            // GET - All
            shops.MapGet("/", async (IShopService service) =>
            {
                var all = await service.GetAllAsync();
                return Results.Ok(all);
            }).RequireAuthorization();

            shops.MapGet("/tabulator", GetPagedShops).RequireAuthorization();

            // GET - By ID
            shops.MapGet("/{id:int}", async (int id, IShopService service) =>
            {
                var result = await service.GetByIdAsync(id);
                return result is null ? Results.NotFound() : Results.Ok(result);
            }).RequireAuthorization();

            //For website without Auth
            shops.MapGet("/all-shops-web", async (IShopService service) =>
            {
                var shops = await service.GetAllShopsForWebSite();
                return Results.Ok(shops);
            });

            //For website searched shop without Auth
            shops.MapGet("/searched-shop-web", async (string? search, IShopService service) =>
            {
                var results = await service.GetSearchedShopsForWeb(search);
                return Results.Ok(results);
            });

            // PUT - Update
            shops.MapPut("/{id:int}", async (int id, [FromBody] ShopDto dto, IShopService service) =>
            {
                var updated = await service.UpdateAsync(id, dto);
                return updated is null ? Results.NotFound() : Results.Ok(updated);
            }).RequireAuthorization();

            // PATCH - STATUS
            shops.MapPatch("/{id}/status", async (int id, [FromBody] short isActive, IShopService service) =>
            {
                var updated = await service.UpdateStatusAsync(id, isActive);
                return updated is null ? Results.NotFound() : Results.Ok(updated);
            }).RequireAuthorization();


            // DELETE
            shops.MapDelete("/{id:int}", async (int id, IShopService service) =>
            {
                var success = await service.DeleteAsync(id);
                return success ? Results.NoContent() : Results.NotFound();
            }).RequireAuthorization();
        }

        private static async Task<IResult> GetPagedShops(HttpRequest req, IShopService service)
        {
            var query = RegexParseFilterSort.BindPagedQueryDto(req.Query);
            var paged = await service.GetAllAsync(query);
            return Results.Ok(paged);
        }
    }
}
