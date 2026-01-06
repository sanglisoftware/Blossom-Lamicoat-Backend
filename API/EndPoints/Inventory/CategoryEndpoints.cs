using Api.Application.DTOs;
using Api.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.API.EndPoints.Inventory
{
    public static class CategoryEndpoints
    {
        public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
        {
            //Categories

            var Categories = app.MapGroup("/api/inventory/Categories");

            Categories.MapGet("/", GetPagedCategories).RequireAuthorization();

            Categories.MapGet("/categories", async (ICategoryService service) =>
            {
                var categories = await service.GetAllCategoriesWebsite();
                return categories is null || !categories.Any() ? Results.NotFound() : Results.Ok(categories);
            });

            Categories.MapGet("/{id:int}", async (int id, ICategoryService service) =>
            {
                var Category = await service.GetByIdAsync(id);
                return Category is null ? Results.NotFound() : Results.Ok(Category);
            }).RequireAuthorization();

            Categories.MapPost("/upload-image", async (IFormFile image, string name, int sequenceNo, int isActive, ICategoryService service) =>
            {
                if (image == null || image.Length == 0) return Results.BadRequest("Image is required");

                var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                var folder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "images",
                    "categories"
                );
                Directory.CreateDirectory(folder);

                var filePath = Path.Combine(folder, fileName);
                await using var stream = new FileStream(filePath, FileMode.Create);
                await image.CopyToAsync(stream);

                var imageUrl = $"/images/categories/{fileName}";

                var dto = new CategoryDto
                {
                    Name = name,
                    SequenceNo = sequenceNo,
                    IsActive = isActive,
                    ImagePath = imageUrl,
                };

                var created = await service.CreateAsync(dto);
                return Results.Created($"/api/inventory/categories/{created.Id}", created);
            }).RequireAuthorization().DisableAntiforgery(); // Required for Swagger in .NET 8

            Categories.MapPut("/update-with-image/{id:int}", async (int id, IFormFile? image, string name, int sequenceNo, int isActive, ICategoryService service) =>
            {
                string? imageUrl = null;

                if (image != null && image.Length > 0)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                    var folder = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "images",
                        "categories"
                    );
                    Directory.CreateDirectory(folder);

                    var filePath = Path.Combine(folder, fileName);
                    await using var stream = new FileStream(filePath, FileMode.Create);
                    await image.CopyToAsync(stream);

                    imageUrl = $"/images/categories/{fileName}";
                }

                var dto = new CategoryDto
                {
                    Id = id,
                    Name = name,
                    SequenceNo = sequenceNo,
                    IsActive = isActive,
                    ImagePath = imageUrl ?? string.Empty, // send empty or preserve existing in service layer
                };

                var updated = await service.UpdateAsync(id, dto);
                return updated is null ? Results.NotFound() : Results.Ok(updated);
            }).RequireAuthorization().DisableAntiforgery();

            Categories.MapPatch("/{id}/sequence", async (int id, [FromBody] int sequenceNo, ICategoryService service) =>
            {
                var existing = await service.GetByIdAsync(id);
                if (existing == null) return Results.NotFound();

                existing.SequenceNo = sequenceNo;

                var updated = await service.UpdateAsync(id, existing);
                return updated is null ? Results.Problem("Failed to update sequence") : Results.Ok(updated);
            }).RequireAuthorization();

            Categories.MapPatch("/{id}/status", async (int id, [FromBody] short isActive, ICategoryService service, IProductService productService) =>
            {
                var updatedCollection = await service.UpdateStatusAsync(id, isActive);
                return updatedCollection is null ? Results.Problem("Failed to update status") : Results.Ok(updatedCollection);
            }).RequireAuthorization();

            Categories.MapDelete("/{id:int}", async (int id, ICategoryService service) =>
            {
                return await service.DeleteAsync(id) ? Results.NoContent() : Results.NotFound();
            }).RequireAuthorization();
        }

        private static async Task<IResult> GetPagedCategories(HttpRequest req, ICategoryService service)
        {
            var query = RegexParseFilterSort.BindPagedQueryDto(req.Query);
            var paged = await service.GetAllAsync(query);
            return Results.Ok(paged);
        }
    }
}
