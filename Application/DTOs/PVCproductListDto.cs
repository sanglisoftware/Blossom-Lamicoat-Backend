namespace Api.Application.DTOs;

public class PVCproductListDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int GramageMasterId { get; set; }

    public int WidthMasterId { get; set; } 

    public int ColourMasterId { get; set; } 

    public string Comments { get; set; }  = string.Empty;

    public short? IsActive { get; set; }



    public string? GramageMasterName { get; set; }

     public string? WidthMasterName { get; set; }

     public string? ColourMasterName { get; set; }


}