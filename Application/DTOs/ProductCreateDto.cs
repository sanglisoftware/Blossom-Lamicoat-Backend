namespace Api.Application.DTOs;
// using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;

public class ProductCreateDto
{
    public string Name { get; set; } = string.Empty;
    public int? CategoryId { get; set; }
    public int? SequenceNo { get; set; }
    public int? ProductCode { get; set; }
    public string Color { get; set; } = string.Empty;
    public int? SizeId { get; set; }

    // Accepting multiple files
    // [FromForm(Name = "Images")]
    public required List<IFormFile> Images { get; set; }

    public string? VideoUrl { get; set; }
    public decimal? Price { get; set; }
    public decimal? Gst { get; set; }
    public string? HsnCode { get; set; }
    public string? ShortDescription { get; set; }
    public string? DetailDescription { get; set; }
    public short? IsStandalone { get; set; }
    public short? IsActive { get; set; }
}
