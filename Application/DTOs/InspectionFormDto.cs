namespace Api.Application.DTOs;

public class InspectionFormDto
{
    public int Id { get; set; }
    public int ManufacturedFabricProductId { get; set; }
    public int GradeId { get; set; }
    public decimal Mtr { get; set; }
    public decimal WastageMtr { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string ManufacturedFabricProductName { get; set; } = string.Empty;
    public string GradeName { get; set; } = string.Empty;
}
