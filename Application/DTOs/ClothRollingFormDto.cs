namespace Api.Application.DTOs;

public class ClothRollingFormDto
{
    public int Id { get; set; }
    public int? FabricInwardId { get; set; }
    public string RollNo { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string Gramage { get; set; } = string.Empty;
    public string Colour { get; set; } = string.Empty;
    public string BatchNo { get; set; } = string.Empty;
    public decimal RollMtr { get; set; }
    public decimal DefectMtr { get; set; }
    public string CheckerName { get; set; } = string.Empty;
    public short? IsActive { get; set; }
    public DateTime? CreatedDate { get; set; }
}
