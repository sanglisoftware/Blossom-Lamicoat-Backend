
namespace Api.Domain.Entities
{
    public class Chemical
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Comment { get; set; }  = string.Empty;
        public short? IsActive { get; set; } 
        
    }
}
