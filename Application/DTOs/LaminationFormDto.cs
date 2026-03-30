namespace Api.Application.DTOs;

public class LaminationFormDto
{
    public int Id { get; set; }
    public int FinalProductId { get; set; }
    public int? ClothRollingFormId { get; set; }
    public string ClothRollCode { get; set; } = string.Empty;
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
    public DateTime? CreatedDate { get; set; }
    public string FinalProductName { get; set; } = string.Empty;
    public string PVCName { get; set; } = string.Empty;
    public string ChemicalName { get; set; } = string.Empty;
    public string WorkerName { get; set; } = string.Empty;
}
