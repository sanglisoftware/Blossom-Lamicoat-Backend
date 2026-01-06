
namespace Api.Domain.Entities
{
    public class Gallery
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int FilterId { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public int SequenceNo { get; set; }
        public int IsActive { get; set; }
        public GalleryFilter? GalleryFilter { get; set; }
    }
}
