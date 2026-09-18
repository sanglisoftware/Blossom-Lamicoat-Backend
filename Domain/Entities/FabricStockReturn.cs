namespace Api.Domain.Entities;

public class FabricStockReturn
{
    public int Id { get; set; }
    public int FabricInwardId { get; set; }
    public double QtyMtr { get; set; }
    public DateTime ReturnDate { get; set; }
    public string? Remarks { get; set; }
    public short? IsActive { get; set; }
    public FabricInward? FabricInward { get; set; }
}
