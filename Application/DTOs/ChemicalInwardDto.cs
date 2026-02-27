namespace Api.Application.DTOs;

public class ChemicalInwardDto
{
    public int Id { get; set; }

    public int ChemicalMasterId { get; set; }

    public double Qty { get; set; }

    public int SupplierMasterId { get; set; }

    public double BatchNo { get; set; }

    public DateTime BillDate { get; set; }

    public DateTime ReceivedDate { get; set; }

    public short? IsActive { get; set; }

    public string? ChemicalMasterName { get; set; }

     public string? SupplierMasterName { get; set; }

}
