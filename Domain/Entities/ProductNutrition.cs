namespace Api.Domain.Entities;

public class ProductNutrition
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string NutritionalValue { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}
