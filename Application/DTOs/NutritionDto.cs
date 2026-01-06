namespace Api.Application.DTOs
{
    public class NutritionRowDto
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string DailyValue { get; set; } = string.Empty;
    }

    public class NutritionDto
    {
        public string Title { get; set; } = string.Empty;
        public string ServingSize { get; set; } = string.Empty;
        public List<NutritionRowDto> Rows { get; set; } = new();
    }

    public class ProductNutritionDto
    {
        public int ProductId { get; set; }
        public NutritionDto Nutrition { get; set; } = new();
    }

    public class NutritionWebsiteResponseDto
    {
        public string? Title { get; set; }
        public string? ServingSize { get; set; }
        public List<NutritionWebsiteResponseRow>? Rows { get; set; }
    }

    public class NutritionWebsiteResponseRow
    {
        public string? Name { get; set; }
        public string? Value { get; set; } // Combined value + unit
        public string? DailyValue { get; set; }
    }
}
