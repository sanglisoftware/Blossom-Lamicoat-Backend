namespace Api.Application.DTOs;

public class FabricInwardDto
{
    public int Id { get; set; }

    public int SupplierMasterId { get; set; }

    public int FabricMasterId { get; set; }

    public double BatchNo { get; set; }

    public double QtyMTR { get; set; }

    public string Comments { get; set; } = string.Empty;

    public short? IsActive { get; set; }

     public string? SupplierMasterName { get; set; }
     
    public string? FabricMasterName { get; set; }


}
