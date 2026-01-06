namespace Api.Application.DTOs;

// Separate the DTOs instead of inheritance
public class ProductUpdateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? CategoryId { get; set; }
    public int? SequenceNo { get; set; }
    public int? ProductCode { get; set; }
    public string Color { get; set; } = string.Empty;
    public int? SizeId { get; set; }
    
    // Make Images optional for updates
  public IFormFileCollection? Images { get; set; }    
    public string? VideoUrl { get; set; }
    public decimal? Price { get; set; }
    public decimal? Gst { get; set; }
    public string? HsnCode { get; set; }
    public string? ShortDescription { get; set; }
    public string? DetailDescription { get; set; }
    public short? IsStandalone { get; set; }
    public short? IsActive { get; set; }
    
    // Optional: Add property to indicate if existing images should be kept
    public bool KeepExistingImages { get; set; } = true;
     public List<string>? ImagesToDelete { get; set; } 
}