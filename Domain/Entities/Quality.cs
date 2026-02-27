
namespace Api.Domain.Entities
{
    public class Quality
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Comments { get; set; }  = string.Empty;
        public int GSMGLMMasterId { get; set; }       
         public int ColourMasterId { get; set; } 
        public short? IsActive { get; set; } 

        public Colour? Colour { get; set; }
        public GSM? GSM { get; set; }
        
    }
}
