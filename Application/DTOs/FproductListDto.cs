namespace Api.Application.DTOs;

public class FproductListDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string GRM { get; set; } = string.Empty;

    public string Colour { get; set; }  = string.Empty;

    public string Comments { get; set; }  = string.Empty;

    public short? IsActive { get; set; }

}