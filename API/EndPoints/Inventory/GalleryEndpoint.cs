using Api.Application.DTOs;
using Api.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.API.EndPoints.Inventory
{
    public static class GalleryEndpoints
    {
        public static void MapGalleryEndpoints(this IEndpointRouteBuilder app)
        {
            var Gallery = app.MapGroup("/api/inventory/Gallery");

            Gallery.MapGet("/", GetPagedGallery).RequireAuthorization();

            //Public API For Website
            Gallery.MapGet("/website", async (IGalleryService service) =>
            {
                var gallery = await service.GetGalleryForWebsite(); // Ensure this is the correct method from the service
                return gallery is null ? Results.NotFound() : Results.Ok(gallery);
            });

            Gallery.MapGet("/{id:int}", async (int id, IGalleryService service) =>
            {
                var gallery = await service.GetByIdAsync(id);
                return gallery is null ? Results.NotFound() : Results.Ok(gallery);
            }).RequireAuthorization();

            Gallery.MapPost("/upload-image", async (IFormFile image, string title, int FilterId, int sequenceNo, int isActive, IGalleryService service) =>
            {
                if (image == null || image.Length == 0) return Results.BadRequest("Image is required");

                var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                var folder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "images",
                    "Gallery"
                );
                Directory.CreateDirectory(folder);

                var filePath = Path.Combine(folder, fileName);
                await using var stream = new FileStream(filePath, FileMode.Create);
                await image.CopyToAsync(stream);

                var imageUrl = $"/images/gallery/{fileName}";

                var dto = new GalleryDto
                {
                    Title = title,
                    FilterId = FilterId,
                    SequenceNo = sequenceNo,
                    IsActive = isActive,
                    ImagePath = imageUrl,
                };

                var created = await service.CreateAsync(dto);
                return Results.Created($"/api/inventory/gallery/{created.Id}", created);
            }).RequireAuthorization().DisableAntiforgery(); // Required for Swagger in .NET 8

            Gallery.MapPut("/update-with-image/{id:int}", async (int id, IFormFile? image, string title, int FilterId, int sequenceNo, int isActive, IGalleryService service) =>
            {
                string? imageUrl = null;

                if (image != null && image.Length > 0)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                    var folder = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "images",
                        "Gallery"
                    );
                    Directory.CreateDirectory(folder);

                    var filePath = Path.Combine(folder, fileName);
                    await using var stream = new FileStream(filePath, FileMode.Create);
                    await image.CopyToAsync(stream);

                    imageUrl = $"/images/gallery/{fileName}";
                }

                var dto = new GalleryDto
                {
                    Id = id,
                    Title = title,
                    FilterId = FilterId,
                    SequenceNo = sequenceNo,
                    IsActive = isActive,
                    ImagePath = imageUrl ?? string.Empty, // send empty or preserve existing in service layer
                };

                var updated = await service.UpdateAsync(id, dto);
                return updated is null ? Results.NotFound() : Results.Ok(updated);
            }).RequireAuthorization().DisableAntiforgery();

            Gallery.MapPatch("/{id}/sequence", async (int id, [FromBody] int sequenceNo, IGalleryService service) =>
            {
                var existing = await service.GetByIdAsync(id);
                if (existing == null) return Results.NotFound();

                existing.SequenceNo = sequenceNo;

                var updated = await service.UpdateAsync(id, existing);
                return updated is null ? Results.Problem("Failed to update sequence") : Results.Ok(updated);
            }).RequireAuthorization();

            Gallery.MapPatch("/{id}/status", async (int id, [FromBody] short isActive, IGalleryService service) =>
            {
                var updatedGallery = await service.UpdateStatusAsync(id, isActive);
                return updatedGallery is null ? Results.Problem("Failed to update status") : Results.Ok(updatedGallery);
            }).RequireAuthorization();

            Gallery.MapDelete("/{id:int}", async (int id, IGalleryService service) =>
            {
                return await service.DeleteAsync(id) ? Results.NoContent() : Results.NotFound();
            }).RequireAuthorization();
        }

        private static async Task<IResult> GetPagedGallery(HttpRequest req, IGalleryService service)
        {
            var query = RegexParseFilterSort.BindPagedQueryDto(req.Query);
            var paged = await service.GetAllAsync(query);
            return Results.Ok(paged);
        }
    }
}
