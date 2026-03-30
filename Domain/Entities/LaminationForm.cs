namespace Api.Domain.Entities;

public class LaminationForm
{
    public int Id { get; set; }
    public int FinalProductId { get; set; }
    public int? ClothRollingFormId { get; set; }
    public string ClothRollBatchNo { get; set; } = string.Empty;
    public int? PVCMasterId { get; set; }
    public string PVCBatchNo { get; set; } = string.Empty;
    public decimal PVCQty { get; set; }
    public int ChemicalId { get; set; }
    public decimal ChemicalQty { get; set; }
    public string Bounding { get; set; } = string.Empty;
    public int WorkerId { get; set; }
    public decimal Temperature { get; set; }
    public string ProcessTime { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }

    public FinalProduct? FinalProduct { get; set; }
    public ClothRollingForm? ClothRollingForm { get; set; }
    public PVCproductList? PVC { get; set; }
    public Chemical? Chemical { get; set; }
    public Employee? Worker { get; set; }
}
