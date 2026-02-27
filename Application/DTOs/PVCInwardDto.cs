namespace Api.Application.DTOs;

public class PVCInwardDto
{
    public int Id { get; set; }

    public int SupplierMasterId { get; set; }

    public int PVCMasterId { get; set; }

    public string New_RollNo { get; set; } = string.Empty;

    public double BatchNo { get; set; }

    public double Qty_kg { get; set; }

    public double Qty_Mtr { get; set; }

    public string Comments { get; set; } = string.Empty;

    public DateTime BillDate { get; set; }

    public DateTime ReceivedDate { get; set; }

    public short? IsActive { get; set; }

    public string? SupplierMasterName { get; set; }

    public string? PVCMasterName { get; set; }


}
