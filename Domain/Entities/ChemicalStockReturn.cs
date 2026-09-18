namespace Api.Domain.Entities;

public class ChemicalStockReturn
{
    public int Id { get; set; }
    public int ChemicalMasterId { get; set; }
    public double Qty { get; set; }
    public DateTime ReturnDate { get; set; }
    public string? Remarks { get; set; }
    public short? IsActive { get; set; }

    public Chemical? Chemical { get; set; }
}
