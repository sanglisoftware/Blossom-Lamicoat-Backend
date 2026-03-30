namespace Api.Application.DTOs;

public class MixtureFormDto
{
    public int Id { get; set; }
    public int FormulaMasterId { get; set; }
    public decimal TotalMixture { get; set; }
    public string MixtureName { get; set; } = string.Empty;
    public DateTime? CreatedDate { get; set; }
    public string FinalProductName { get; set; } = string.Empty;
}
