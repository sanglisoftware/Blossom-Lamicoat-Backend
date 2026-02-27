
namespace Api.Domain.Entities
{
    public class FGramage
    {
        public int Id { get; set; }
        public string GRM { get; set; } = string.Empty;
        public short? IsActive { get; set; } 

          public ICollection<FproductList> Fgrmage
        { get; set; } = new List<FproductList>(); 
        
    }
}
