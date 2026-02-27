namespace Api.Domain.Entities
{
    public class ChemicalInward
    {
        public int Id { get; set; }

        public int ChemicalMasterId { get; set; }

        public double Qty { get; set; }

        public int SupplierMasterId { get; set; }

        public double BatchNo { get; set; }

        public DateTime BillDate { get; set; }

        public DateTime ReceivedDate { get; set; }

        public short? IsActive { get; set; } 
        
        public Chemical? Chemical { get; set; }

        public Supplier? Supplier { get; set; }

    }
}
