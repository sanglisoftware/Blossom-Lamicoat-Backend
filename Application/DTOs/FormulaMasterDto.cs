namespace Api.Application.DTOs;

public class FormulaMasterDto
{
    public int Id { get; set; }

    public int FinalProductId { get; set; }

    public string MixtureName { get; set; } = string.Empty;

    public string? Final_Product { get; set; }

    public short? IsActive { get; set; }
}
