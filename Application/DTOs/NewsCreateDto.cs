namespace Api.Application.DTOs;

public class NewsCreateDto
{
    public int Id { get; set; }
    public string? Href { get; set; }
    public string? Card { get; set; }
    public string? Card2 { get; set; }
    public string? Title { get; set; }
    public string? Para { get; set; }
    public int? Views { get; set; }
    public int? Comment { get; set; }
    public string? Date { get; set; }
    public string? Video { get; set; }
    public short IsActive { get; set; }
    public string? Img { get; set; } = string.Empty;
}
