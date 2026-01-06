namespace Api.Domain.Entities;

public class Menu
{
    public int Id { get; set; }
    public int? ParentId { get; set; }
    public string Icon { get; set; } = string.Empty;
    public string? Pathname { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Sequence { get; set; } = 0;
}