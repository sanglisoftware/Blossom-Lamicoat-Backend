
namespace Api.Domain.Entities
{
    public class Quality
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Comments { get; set; }  = string.Empty;
        public string GSM_GLM { get; set; } = string.Empty;
        public string Colour { get; set; }  = string.Empty;
        public short? IsActive { get; set; } 
        
    }
}
