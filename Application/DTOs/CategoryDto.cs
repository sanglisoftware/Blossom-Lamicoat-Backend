namespace Api.Application.DTOs;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ImagePath { get; set; }= string.Empty;
    public int SequenceNo { get; set; }
    public int IsActive { get; set; }
}
