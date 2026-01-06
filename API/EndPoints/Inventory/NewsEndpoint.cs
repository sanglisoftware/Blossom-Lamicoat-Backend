using Api.Application.DTOs;
using Api.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.API.EndPoints.Inventory
{
    public static class NewsEndpoint
    {
        public static void MapNewsEndpoints(this IEndpointRouteBuilder app)
        {
            var News = app.MapGroup("/api/inventory/news");

            News.MapPost("/upload", async (HttpContext context, INewsService service) =>
            {
                try
                {
                    var form = await context.Request.ReadFormAsync();
                    var imageFiles = form.Files.Where(f => f.Name == "Images").ToList();

                    if (imageFiles.Count == 0)
                    {
                        return Results.BadRequest("At least one image is required");
                    }

                    // Process file uploads
                    var uploadDir = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "images",
                        "news"
                    );
                    Directory.CreateDirectory(uploadDir);

                    var imagePaths = new List<string>();
                    foreach (var file in imageFiles)
                    {
                        var fileName =
                            $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                        var fullPath = Path.Combine(uploadDir, fileName);

                        await using var stream = new FileStream(fullPath, FileMode.Create);
                        await file.CopyToAsync(stream);

                        imagePaths.Add($"/images/news/{fileName}");
                    }

                    // Create DTO with required fields only
                    var newsDto = new NewsCreateDto
                    {
                        Href = form["href"],
                        Card = form["card"],
                        Card2 = form["card2"],
                        Title = form["title"],
                        Para = form["para"],
                        //IsActive =
                        Img = string.Join(",", imagePaths),
                        Date = form["date"],
                        Video = form["video"],
                        Views = int.TryParse(form["views"], out var views) ? views : null,
                        Comment = int.TryParse(form["comment"], out var comment)
                            ? comment
                            : null,
                    };

                    var created = await service.AddNewsAsync(newsDto);
                    return Results.Created($"/api/inventory/news/{created.Id}", created);
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        title: "Error uploading News",
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError
                    );
                }
            }).DisableAntiforgery().RequireAuthorization();

            News.MapGet("/", async (HttpRequest req, INewsService service) =>
            {
                var query = RegexParseFilterSort.BindPagedQueryDto(req.Query);
                var paged = await service.GetAllAsync(query);
                return Results.Ok(paged);
            });//.RequireAuthorization();

            News.MapGet("/{id:int}", async (int id, INewsService service) =>
            {
                var news = await service.GetByIdAsync(id);
                return news is null ? Results.NotFound() : Results.Ok(news);
            }).RequireAuthorization();

            //News.MapPut(
            //        "/update-with-image/{id:int}",
            //        async (
            //            int id,
            //            IFormFile? image,
            //            string? href,
            //            string? card,
            //            string? card2,
            //            string? title,
            //            string? para,
            //            int? views,
            //            int? comment,
            //            string? date,
            //            string? video,
            //            short isActive,
            //            INewsService service
            //        ) =>
            //        {
            //            string? imagePath = null;

            //            if (image != null && image.Length > 0)
            //            {
            //                var uploadDir = Path.Combine(
            //                    Directory.GetCurrentDirectory(),
            //                    "wwwroot",
            //                    "images",
            //                    "news"
            //                );
            //                Directory.CreateDirectory(uploadDir);

            //                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
            //                var fullPath = Path.Combine(uploadDir, fileName);

            //                await using var stream = new FileStream(fullPath, FileMode.Create);
            //                await image.CopyToAsync(stream);

            //                imagePath = $"/images/news/{fileName}";
            //            }

            //            var dto = new NewsCreateDto
            //            {
            //                Id = id,
            //                Href = href,
            //                Card = card,
            //                Card2 = card2,
            //                Title = title,
            //                Para = para,
            //                Views = views,
            //                Comment = comment,
            //                Date = date,
            //                Video = video,
            //                IsActive = isActive,
            //                Img = imagePath ?? string.Empty,
            //            };

            //            var updated = await service.UpdateAsync(id, dto);
            //            return updated is null ? Results.NotFound() : Results.Ok(updated);
            //        }
            //    )
            //    .RequireAuthorization()
            //    .DisableAntiforgery();

            News.MapPut("/update-with-image/{id:int}", async (HttpContext context, int id, INewsService service) =>
            {
                try
                {
                    var form = await context.Request.ReadFormAsync();
                    var image = form.Files["image"];
                    var removedImagesJson = form["removedImages"];

                    string? imagePath = null;

                    if (image != null && image.Length > 0)
                    {
                        var uploadDir = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot",
                            "images",
                            "news"
                        );
                        Directory.CreateDirectory(uploadDir);

                        var fileName =
                            $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                        var fullPath = Path.Combine(uploadDir, fileName);

                        await using var stream = new FileStream(fullPath, FileMode.Create);
                        await image.CopyToAsync(stream);

                        imagePath = $"/images/news/{fileName}";
                    }

                    var dto = new NewsCreateDto
                    {
                        Id = id,
                        Href = form["href"],
                        Card = form["card"],
                        Card2 = form["card2"],
                        Title = form["title"],
                        Para = form["para"],
                        Views = int.TryParse(form["views"], out var views) ? views : null,
                        Comment = int.TryParse(form["comment"], out var comment)
                            ? comment
                            : null,
                        Date = form["date"],
                        Video = form["video"],
                        IsActive = short.TryParse(form["isActive"], out var isActive)
                            ? isActive
                            : (short)1,
                        Img = imagePath ?? string.Empty,
                    };

                    // You can deserialize removedImagesJson if needed:
                    // var removedImages = JsonSerializer.Deserialize<List<string>>(removedImagesJson);

                    var updated = await service.UpdateAsync(id, dto);
                    return updated is null ? Results.NotFound() : Results.Ok(updated);
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        title: "Error updating News",
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError
                    );
                }
            }).RequireAuthorization().DisableAntiforgery();

            News.MapPatch("/{id}/status", async (int id, [FromBody] short isActive, INewsService service) =>
            {
                var updated = await service.UpdateStatusAsync(id, (short)isActive);
                return updated is null ? Results.NotFound() : Results.Ok(updated);
            }).RequireAuthorization();

            News.MapDelete("/{id:int}", async (int id, INewsService service) =>
            {
                return await service.DeleteAsync(id) ? Results.NoContent() : Results.NotFound();
            }).RequireAuthorization();
        }
    }
}
