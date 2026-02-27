namespace Api.Application.DTOs;

public class QualityDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Comments { get; set; } = string.Empty;

    public int GSMGLMMasterId { get; set; }

    public int ColourMasterId { get; set; }

    public short? IsActive { get; set; }


    public string? GSMMasterName { get; set; }

     public string? ColourMasterName { get; set; }

}