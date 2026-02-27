
namespace Api.Domain.Entities
{
    public class Gramage
    {
        public int Id { get; set; }
        public string GRM { get; set; } = string.Empty;
        public short? IsActive { get; set; } 


          public ICollection<PVCproductList> PVCGramage
        { get; set; } = new List<PVCproductList>();   
        
    }
}
