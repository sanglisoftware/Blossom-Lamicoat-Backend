using Api.Application.DTOs;
using Api.Application.Interfaces;
using Api.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace Api.API.EndPoints.Inventory
{
    public static class ProductEndpoints
    {
        private static async Task<IResult> GetPagedProducts(HttpRequest req, IProductService service)
        {
            var query = RegexParseFilterSort.BindPagedQueryDto(req.Query);
            var paged = await service.GetAllAsync(query);
            return Results.Ok(paged);
        }

        // Utility to parse filter[0][…], sort[0][…], page & size
        public static void MapProductEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/inventory/products");

            group.MapGet("/", GetPagedProducts).RequireAuthorization();

            group.MapGet("/featured-web", async (IProductService service) =>
            {
                var products = await service.GetAllFeaturedProductsWebsite();
                return products is null || products.Count == 0 ? Results.NotFound() : Results.Ok(products);
            });

            group.MapGet("/products-web", async (HttpContext context, IProductService service) =>
            {
                var query = context.Request.Query;

                _ = int.TryParse(query["id"], out int id);

                _ = int.TryParse(query["category"], out int category);
                string? search = query["search"];
                _ = int.TryParse(query["pagesize"], out int pageSize);
                _ = int.TryParse(query["page"], out int page);

                var products = await service.GetAllProductsWebsite(id, category, search, pageSize, page);
                return products is null ? Results.NotFound() : Results.Ok(products);
            });

            group.MapGet("/related-products-web/{productId:int}", async (int productId, IProductService service) =>
            {
                var products = await service.GetRelatedProductsWebsite(productId);
                return products is null || products.Count == 0 ? Results.NotFound() : Results.Ok(products);
            });

            group.MapGet("/{id:int}", async (int id, IProductService service) =>
            {
                var result = await service.GetByIdAsync(id);
                return result is null ? Results.NotFound() : Results.Ok(result);
            }).RequireAuthorization();

            group.MapDelete("/{id:int}", async (int id, IProductService service) => await service.DeleteAsync(id) ? Results.NoContent() : Results.NotFound()).RequireAuthorization();

            group.MapPost("/test", async (AppDbContext dbContext) =>
            {
                try
                {
                    var product = new Domain.Entities.Product { Name = "Test Product" };

                    dbContext.Products.Add(product);
                    await dbContext.SaveChangesAsync();

                    return Results.Ok(new { message = "Test product created successfully", id = product.Id });
                }
                catch (Exception ex)
                {
                    // Enhanced error details
                    return Results.Problem(
                        title: "Test failed",
                        detail: $"Message: {ex.Message}\n"
                            + $"Inner Exception: {ex.InnerException?.Message}\n"
                            + $"Stack Trace: {ex.StackTrace}",
                        statusCode: StatusCodes.Status500InternalServerError
                    );
                }
            }
            );

            group.MapPost("/upload", async (HttpContext context, IProductService service) =>
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
                        "products"
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

                        imagePaths.Add($"/images/products/{fileName}");
                    }

                    // Create DTO with required fields only
                    var productDto = new ProductDto
                    {
                        // ID is NOT SET here - will be generated by DB
                        Name = form["Name"].ToString(),
                        CategoryId = int.TryParse(form["CategoryId"], out var catId)
                            ? catId
                            : null,
                        SizeId = int.TryParse(form["SizeId"], out var sizeId)
                            ? sizeId
                            : null,
                        SequenceNo = int.TryParse(form["SequenceNo"], out var seqNo)
                            ? seqNo
                            : null,
                        ProductCode = int.TryParse(form["ProductCode"], out var prodCode)
                            ? prodCode
                            : null,
                        Color = form["Color"].ToString(),
                        VideoUrl = form["VideoUrl"],
                        Price = decimal.TryParse(form["Price"], out var price)
                            ? price
                            : null,
                        Gst = decimal.TryParse(form["Gst"], out var gst) ? gst : null,
                        HsnCode = form["HsnCode"],
                        ShortDescription = form["ShortDescription"],
                        DetailDescription = form["DetailDescription"],
                        IsStandalone = short.TryParse(
                            form["IsStandalone"],
                            out var isStandalone
                        )
                            ? isStandalone
                            : (short?)null,
                        IsActive = short.TryParse(form["IsActive"], out var isActive)
                            ? isActive
                            : (short?)null,
                        ImagePaths = string.Join(",", imagePaths),
                    };

                    var created = await service.CreateAsync(productDto);
                    return Results.Created(
                        $"/api/inventory/products/{created.Id}",
                        created
                    );
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        title: "Error uploading product",
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError
                    );
                }
            }).DisableAntiforgery().RequireAuthorization();

            group.MapPut("/update-with-image/{id:int}", async (int id, [FromForm] ProductUpdateDto dto, IProductService service) =>
            {
                try
                {
                    var existingProduct = await service.GetByIdAsync(id);
                    if (existingProduct == null)
                    {
                        return Results.NotFound();
                    }

                    var imagePaths = new List<string>();

                    // 1. Process existing images - filter out deleted ones
                    if (!string.IsNullOrEmpty(existingProduct.ImagePaths))
                    {
                        var existingImageList = existingProduct.ImagePaths.Split(',');

                        // Filter out images that should be deleted
                        foreach (var existingImage in existingImageList)
                        {
                            // If client sent "ImagesToDelete" with this path, skip it
                            if (
                                dto.ImagesToDelete != null
                                && dto.ImagesToDelete.Contains(existingImage)
                            )
                            {
                                // Delete the physical file
                                var filePath = Path.Combine(
                                    Directory.GetCurrentDirectory(),
                                    "wwwroot",
                                    existingImage.TrimStart('/')
                                );
                                if (File.Exists(filePath))
                                {
                                    File.Delete(filePath);
                                }
                            }
                            else
                            {
                                // Keep the image
                                imagePaths.Add(existingImage);
                            }
                        }
                    }

                    // 2. Process new images
                    if (dto.Images?.Count > 0)
                    {
                        var uploadDir = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot",
                            "images",
                            "products"
                        );
                        Directory.CreateDirectory(uploadDir);

                        foreach (var file in dto.Images)
                        {
                            if (file.Length == 0)
                                continue;

                            var fileName =
                                $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                            var fullPath = Path.Combine(uploadDir, fileName);

                            await using var stream = new FileStream(
                                fullPath,
                                FileMode.Create
                            );
                            await file.CopyToAsync(stream);

                            imagePaths.Add($"/images/products/{fileName}");
                        }
                    }

                    // 3. Create DTO for update
                    var productDto = new ProductDto
                    {
                        Id = id,
                        Name = dto.Name,
                        CategoryId = dto.CategoryId,
                        SizeId = dto.SizeId,
                        SequenceNo = dto.SequenceNo,
                        ProductCode = dto.ProductCode,
                        Color = dto.Color,
                        VideoUrl = dto.VideoUrl,
                        Price = dto.Price,
                        Gst = dto.Gst,
                        HsnCode = dto.HsnCode,
                        ShortDescription = dto.ShortDescription,
                        DetailDescription = dto.DetailDescription,
                        IsStandalone = dto.IsStandalone,
                        IsActive = dto.IsActive,
                        ImagePaths = string.Join(",", imagePaths),
                    };

                    var updated = await service.UpdateAsync(id, productDto);
                    return updated is null ? Results.NotFound() : Results.Ok(updated);
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        title: "Error updating product",
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError
                    );
                }
            }).DisableAntiforgery().RequireAuthorization();

            // Add test update endpoint
            group.MapPut("/test-update/{id:int}", async (int id, AppDbContext dbContext) =>
            {
                try
                {
                    var product = await dbContext.Products.FindAsync(id);
                    if (product == null)
                    {
                        return Results.NotFound();
                    }

                    // Update some fields
                    product.Name = "Updated " + product.Name;
                    product.Price = product.Price * 1.1m; // Increase price by 10%

                    await dbContext.SaveChangesAsync();

                    return Results.Ok(
                        new { message = "Test product updated successfully", id = product.Id }
                    );
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        title: "Test update failed",
                        detail: $"Message: {ex.Message}\nInner: {ex.InnerException?.Message}",
                        statusCode: StatusCodes.Status500InternalServerError
                    );
                }
            }
            );

            group.MapPatch("/{id}/sequence", async (int id, [FromBody] int sequenceNo, IProductService service) =>
            {
                var existing = await service.GetByIdAsync(id);
                if (existing == null) return Results.NotFound();

                existing.SequenceNo = sequenceNo;

                var updated = await service.UpdateAsync(id, existing);
                return updated is null ? Results.Problem("Failed to update sequence") : Results.Ok(updated);
            }).RequireAuthorization();

            group.MapPatch("/{id}/status", async (int id, [FromBody] short isActive, IProductService service) =>
            {
                var updatedProduct = await service.UpdateStatusAsync(id, isActive);
                return updatedProduct is null ? Results.Problem("Failed to update status") : Results.Ok(updatedProduct);
            }).RequireAuthorization();
        }
    }
}
