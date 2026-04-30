
namespace Api.Domain.Entities
{
    public class PVCproductList
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;
        public short? IsActive { get; set; }

        public ICollection<PVCInward> pvcInwards { get; set; } = new List<PVCInward>();
    }
}
