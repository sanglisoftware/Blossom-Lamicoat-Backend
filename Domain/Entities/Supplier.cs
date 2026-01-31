
namespace Api.Domain.Entities
{
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Mobile_No { get; set; }  = string.Empty;
        public string Pan { get; set; }  = string.Empty;
        public string GST_No { get; set; }  = string.Empty;
        public short? IsActive { get; set; } 
        
    }
}
