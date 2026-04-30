namespace Api.Application.DTOs;

public class PVCInwardDto
{
    public int Id { get; set; }

    public int SupplierMasterId { get; set; }

    public int PVCMasterId { get; set; }

    public string? New_RollNo { get; set; }

    public double BatchNo { get; set; }

    public double Qty_kg { get; set; }

    public double Qty_Mtr { get; set; }

    public string? Comments { get; set; }

    public int? GramageMasterId { get; set; }

    public string? GramageName { get; set; }

    public int? WidthMasterId { get; set; }

    public string? WidthName { get; set; }

    public int? ColourMasterId { get; set; }

    public string? ColourName { get; set; }

    public DateTime? BillDate { get; set; }

    public DateTime? ReceivedDate { get; set; }

    public string? AttachedFile { get; set; }

    public short? IsActive { get; set; }

    public string? SupplierMasterName { get; set; }

    public string? PVCMasterName { get; set; }


}
