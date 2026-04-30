
namespace Api.Domain.Entities
{
    public class Colour
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public short? IsActive { get; set; }

        public ICollection<FproductList> FColour
        { get; set; } = new List<FproductList>(); 
        
        public ICollection<Quality> colourquality
        { get; set; } = new List<Quality>(); 
    }
}
