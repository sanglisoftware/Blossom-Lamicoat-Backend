using Api.Application.DTOs;
using Api.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.API.EndPoints.Inventory
{
    public static class SliderEndpoints
    {
        public static void MapSliderEndpoints(this IEndpointRouteBuilder app)
        {
            var Slider = app.MapGroup("/api/inventory/Slider");

            Slider.MapGet("/", GetPagedSlides).RequireAuthorization();

            //Public Api For Website
            Slider.MapGet("/website", async (ISliderService service) =>
            {
                var slides = await service.GetSliderImagesForWebsite();
                return slides is null || !slides.Any() ? Results.NotFound() : Results.Ok(slides);
            });

            Slider.MapGet("/{id:int}", async (int id, ISliderService service) =>
            {
                var slider = await service.GetByIdAsync(id);
                return slider is null ? Results.NotFound() : Results.Ok(slider);
            }).RequireAuthorization();

            Slider.MapPost("/upload-image", async (IFormFile image, int sequenceNo, int isActive, ISliderService service) =>
            {
                if (image == null || image.Length == 0) return Results.BadRequest("Image is required");

                var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                var folder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "images",
                    "Slides"
                );
                Directory.CreateDirectory(folder);

                var filePath = Path.Combine(folder, fileName);
                await using var stream = new FileStream(filePath, FileMode.Create);
                await image.CopyToAsync(stream);

                var imageUrl = $"/images/Slides/{fileName}";

                var dto = new SliderDto
                {
                    SequenceNo = sequenceNo,
                    IsActive = isActive,
                    ImagePath = imageUrl,
                };

                var created = await service.CreateAsync(dto);
                return Results.Created($"/api/inventory/Slides/{created.Id}", created);
            }).RequireAuthorization().DisableAntiforgery(); // Required for Swagger in .NET 8

            Slider.MapPut("/update-with-image/{id:int}", async (int id, IFormFile? image, int sequenceNo, int isActive, ISliderService service) =>
            {
                string? imageUrl = null;

                if (image != null && image.Length > 0)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                    var folder = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "images",
                        "Slides"
                    );
                    Directory.CreateDirectory(folder);

                    var filePath = Path.Combine(folder, fileName);
                    await using var stream = new FileStream(filePath, FileMode.Create);
                    await image.CopyToAsync(stream);

                    imageUrl = $"/images/Slides/{fileName}";
                }

                var dto = new SliderDto
                {
                    Id = id,
                    SequenceNo = sequenceNo,
                    IsActive = isActive,
                    ImagePath = imageUrl ?? string.Empty, // send empty or preserve existing in service layer
                };

                var updated = await service.UpdateAsync(id, dto);
                return updated is null ? Results.NotFound() : Results.Ok(updated);
            }).RequireAuthorization().DisableAntiforgery();

            Slider.MapPatch("/{id}/sequence", async (int id, [FromBody] int sequenceNo, ISliderService service) =>
            {
                var existing = await service.GetByIdAsync(id);
                if (existing == null) return Results.NotFound();

                existing.SequenceNo = sequenceNo;

                var updated = await service.UpdateAsync(id, existing);
                return updated is null ? Results.Problem("Failed to update sequence") : Results.Ok(updated);
            }).RequireAuthorization();

            Slider.MapPatch("/{id}/status", async (int id, [FromBody] short isActive, ISliderService service) =>
            {
                var updatedSlide = await service.UpdateStatusAsync(id, isActive);
                return updatedSlide is null ? Results.Problem("Failed to update status") : Results.Ok(updatedSlide);
            }).RequireAuthorization();

            Slider.MapDelete("/{id:int}", async (int id, ISliderService service) =>
            {
                return await service.DeleteAsync(id) ? Results.NoContent() : Results.NotFound();
            }).RequireAuthorization();
        }

        private static async Task<IResult> GetPagedSlides(HttpRequest req, ISliderService service)
        {
            var query = RegexParseFilterSort.BindPagedQueryDto(req.Query);
            var paged = await service.GetAllAsync(query);
            return Results.Ok(paged);
        }
    }
}
