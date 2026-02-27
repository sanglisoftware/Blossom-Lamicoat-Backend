namespace Api.Domain.Entities
{
    public class FormulaChemicalTransaction
    {
        public int Id { get; set; }

        public int FormulaMasterId { get; set; }

        public int ChemicalMasterId { get; set; }

        public double Qty { get; set; }
        

        public FormulaMaster? FormulaMaster { get; set; }

        public Chemical? Chemical { get; set; }
    }
}
