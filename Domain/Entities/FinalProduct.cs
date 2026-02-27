namespace Api.Domain.Entities
{
    public class FinalProduct
    {
        public int Id { get; set; }
        public string Final_Product { get; set; } = string.Empty;
        public short? IsActive { get; set; }
    }
}
