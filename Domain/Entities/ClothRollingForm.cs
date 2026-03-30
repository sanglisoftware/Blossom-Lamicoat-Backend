namespace Api.Domain.Entities;

public class ClothRollingForm
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string BatchNo { get; set; } = string.Empty;
    public decimal RollMtr { get; set; }
    public decimal DefectMtr { get; set; }
    public string CheckerName { get; set; } = string.Empty;
    public short? IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
}
