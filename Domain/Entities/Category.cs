
namespace Api.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public int SequenceNo { get; set; }
        public int IsActive { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}
