
namespace Api.Domain.Entities
{
    public class PVCproductList
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int GramageMasterId { get; set; } 
        public int WidthMasterId { get; set; }  
        public int ColourMasterId { get; set; } 
        public string Comments { get; set; }  = string.Empty;
        public short? IsActive { get; set; } 

        public Width? Width { get; set; }
        public Colour? Colour { get; set; }
        public Gramage? Gramage { get; set; }

            public ICollection<PVCInward> pvcInwards
            { get; set; } = new List<PVCInward>();
        
    }
}
