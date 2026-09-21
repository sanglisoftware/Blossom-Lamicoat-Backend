namespace Api.Domain.Entities;

public class InspectionForm
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
    public DateTime CreatedDate { get; set; }

    public FproductList? ManufacturedFabricProduct { get; set; }
    public Grade? Grade { get; set; }
    public LaminationForm? LaminationForm { get; set; }
    public FinalProduct? FinalProduct { get; set; }
}
