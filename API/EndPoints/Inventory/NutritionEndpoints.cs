// Api/Endpoints/ProductEndpoints/NutritionEndpoints.cs
using Api.Application.DTOs;
using Api.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.API.EndPoints.Inventory
{
    public static class NutritionEndpoints
    {
        public static void MapNutritionEndpoints(this IEndpointRouteBuilder app)
        {
            var nutritionGroup = app.MapGroup("/api/products/nutrition").RequireAuthorization();

            // Save nutrition data
            nutritionGroup.MapPost("/", async ([FromBody] ProductNutritionDto dto, INutritionService service) =>
            {
                await service.SaveNutritionDataAsync(dto);
                return Results.Ok(new { message = "Nutrition data saved successfully" });
            });

            // Get nutrition data
            nutritionGroup.MapGet("/{productId:int}", async (int productId, INutritionService service) =>
            {
                var nutrition = await service.GetNutritionDataAsync(productId);
                return nutrition == null ? Results.NotFound() : Results.Ok(nutrition);
            });

            var nutritionGroupWebsite = app.MapGroup("/api/products/nutrition");

            // Get nutrition data
            nutritionGroupWebsite.MapGet("website/{productId:int}", async (int productId, INutritionService service) =>
            {
                var nutrition = await service.GetWebsiteNutritionDataAsync(productId);
                return nutrition == null ? Results.NotFound() : Results.Ok(nutrition);
            });
        }
    }
}
