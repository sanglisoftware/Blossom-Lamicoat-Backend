using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Domain.Entities;

public class FormulaMaster
{
    public int Id { get; set; }

    public short? IsActive { get; set; }

    public int FinalProductId { get; set; }

    public FinalProduct? FinalProduct { get; set; }


    [NotMapped]
    public string? Final_Product { get; set; }

    public ICollection<FormulaChemicalTransaction> formulaChemicalTransactions
    { get; set; } = new List<FormulaChemicalTransaction>();
}
