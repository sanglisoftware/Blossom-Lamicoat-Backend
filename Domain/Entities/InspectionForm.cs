namespace Api.Domain.Entities;

public class InspectionForm
{
    public int Id { get; set; }
    public int ManufacturedFabricProductId { get; set; }
    public int GradeId { get; set; }
    public decimal Mtr { get; set; }
    public decimal WastageMtr { get; set; }
    public DateTime CreatedDate { get; set; }

    public FproductList? ManufacturedFabricProduct { get; set; }
    public Grade? Grade { get; set; }
}
