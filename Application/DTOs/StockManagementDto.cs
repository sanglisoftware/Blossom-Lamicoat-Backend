namespace Api.Application.DTOs;

public class ChemicalStockDto
{
    public int ChemicalMasterId { get; set; }
    public string ChemicalName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public double Received { get; set; }
    public double Used { get; set; }
    public double Returned { get; set; }
    public double Balance { get; set; }
}

public class RawMaterialStockDto
{
    public int MasterId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string Gramage { get; set; } = string.Empty;
    public string Colour { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public int RollCount { get; set; }
    public double Received { get; set; }
    public double Used { get; set; }
    public double Returned { get; set; }
    public double Balance { get; set; }
    public double Actual { get; set; }
    public double Defective { get; set; }
    public double Variance { get; set; }
}

public class StockManagementDto
{
    public List<ChemicalStockDto> Chemicals { get; set; } = [];
    public List<RawMaterialStockDto> Mixtures { get; set; } = [];
    public List<RawMaterialStockDto> Fabrics { get; set; } = [];
    public List<RawMaterialStockDto> PVC { get; set; } = [];
    public List<RawMaterialStockDto> FinishedGoods { get; set; } = [];
    public List<RawMaterialStockDto> FinishedRolls { get; set; } = [];
}

public class CreateChemicalStockReturnDto
{
    public int ChemicalMasterId { get; set; }
    public double Qty { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string? Remarks { get; set; }
}

public class CreateFabricStockReturnDto
{
    public int FabricInwardId { get; set; }
    public double QtyMtr { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string? Remarks { get; set; }
}
