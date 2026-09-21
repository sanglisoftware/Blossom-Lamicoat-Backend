namespace Api.Application.DTOs;

public class InspectionFormDto
{
    public int Id { get; set; }
    public int? ManufacturedFabricProductId { get; set; }
    public int? LaminationFormId { get; set; }
    public int? FinalProductId { get; set; }
    public string RollNo { get; set; } = string.Empty;
    public string RollType { get; set; } = "Roll";
    public int GradeId { get; set; }
    public decimal Mtr { get; set; }
    public decimal WastageMtr { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string ManufacturedFabricProductName { get; set; } = string.Empty;
    public string GradeName { get; set; } = string.Empty;
    public string FinalProductName { get; set; } = string.Empty;
    public decimal LaminationQtyMtr { get; set; }
}
