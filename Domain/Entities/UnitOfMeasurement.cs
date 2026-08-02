namespace Api.Domain.Entities
{
    public class UnitOfMeasurement
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public short? IsActive { get; set; }

        public ICollection<ChemicalInward> ChemicalInwards { get; set; } = new List<ChemicalInward>();
    }
}
