
namespace Api.Domain.Entities
{
    public class Slider
    {
        public int Id { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public int SequenceNo { get; set; }
        public int IsActive { get; set; }

    }
}
