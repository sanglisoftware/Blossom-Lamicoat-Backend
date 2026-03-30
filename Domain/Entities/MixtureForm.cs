namespace Api.Domain.Entities;

public class MixtureForm
{
    public int Id { get; set; }
    public int FormulaMasterId { get; set; }
    public decimal TotalMixture { get; set; }
    public string? MixtureName { get; set; }
    public DateTime CreatedDate { get; set; }

    public FormulaMaster? FormulaMaster { get; set; }
}
