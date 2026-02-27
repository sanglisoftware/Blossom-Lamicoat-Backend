
namespace Api.Domain.Entities
{
    public class FproductList
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
       public int FGramageMasterId { get; set; } 
        public int ColourMasterId { get; set; } 
        public string Comments { get; set; }  = string.Empty;
        public short? IsActive { get; set; } 

        public Colour? Colour { get; set; }
        public FGramage? FGramage { get; set; }

            public ICollection<FabricInward> fabricInwards
            { get; set; } = new List<FabricInward>();

        
    }
}
