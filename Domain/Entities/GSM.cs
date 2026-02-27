
namespace Api.Domain.Entities
{
    public class GSM
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public short? IsActive { get; set; } 

        public ICollection<Quality> gsmquality
        { get; set; } = new List<Quality>(); 
        
    }
}
