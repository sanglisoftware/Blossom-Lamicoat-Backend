namespace Api.Application.DTOs;

public class GalleryDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int FilterId { get; set; }
    public string ImagePath { get; set; } = string.Empty;
    public int SequenceNo { get; set; }
    public int IsActive { get; set; }

    public string? FilterName { get; set; } = string.Empty;


}