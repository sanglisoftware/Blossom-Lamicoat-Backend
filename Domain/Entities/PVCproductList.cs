
namespace Api.Domain.Entities
{
    public class PVCproductList
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Gramage { get; set; } = string.Empty;
        public string Width { get; set; }  = string.Empty;
        public string Colour { get; set; }  = string.Empty;
        public string Comments { get; set; }  = string.Empty;
        public short? IsActive { get; set; } 
        
    }
}
