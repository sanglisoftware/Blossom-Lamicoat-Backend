namespace Api.Application.DTOs;

public class FormulaChemicalTransactionDto
{
    public int Id { get; set; }

    public int FormulaMasterId { get; set; }

    public int ChemicalMasterId { get; set; }

    public double Qty { get; set; }

    public string? FinalProductName { get; set; }

    public string? ChemicalMasterName { get; set; }

}
