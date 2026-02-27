public class FormulaDetailsDto
{
    public int FormulaMasterId { get; set; }
    public int FinalProductId { get; set; }
    public List<ChemicalItemDto> Chemicals { get; set; }
}

public class ChemicalItemDto
{
    public int ChemicalMasterId { get; set; }
    public double Qty { get; set; }

     public string ChemicalName { get; set; }
}

public class FormulaTransactionDetailsDto
{
    public int Id { get; set; }
    public int FormulaMasterId { get; set; }
    public int FinalProductId { get; set; }
    public List<ChemicalItemDto> Chemicals { get; set; }
}
