namespace Api.Application.DTOs;

public class ChemicalDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string Comment { get; set; } = string.Empty;


    public short? IsActive { get; set; }

}