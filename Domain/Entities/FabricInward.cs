namespace Api.Domain.Entities
{
    public class FabricInward
    {
        public int Id { get; set; }
        public int SupplierMasterId { get; set; }
        public int FabricMasterId { get; set; }
        public int? FGramageMasterId { get; set; }
        public int? ColourMasterId { get; set; }
        public double BatchNo { get; set; }
        public double QtyMTR { get; set; }
        public string Comments { get; set; } = string.Empty;
        public string? AttachedFile { get; set; }
        public short? IsActive { get; set; }

        public FproductList? Fabric { get; set; }
        public Supplier? Supplier { get; set; }
        public FGramage? FGramage { get; set; }
        public Colour? Colour { get; set; }
    }
}
