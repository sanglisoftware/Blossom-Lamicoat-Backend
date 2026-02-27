namespace Api.Application.DTOs;

public class FproductListDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int FGramageMasterId { get; set; } 

    public int ColourMasterId { get; set; } 

    public string Comments { get; set; }  = string.Empty;

    public short? IsActive { get; set; }



    public string? FGramageMasterName { get; set; }

     public string? ColourMasterName { get; set; }

}