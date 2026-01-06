namespace Api.Domain.Entities;

public class GalleryFilter
{
    public int Id { get; set; } // Auto-increment
    public string FilterValue { get; set; } = string.Empty;
}
