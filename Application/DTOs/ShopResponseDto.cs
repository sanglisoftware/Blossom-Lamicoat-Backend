using System.Text.Json.Serialization;

namespace Api.Application.DTOs;

public class ShopResponseDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Href { get; set; }
    public string? Mobile { get; set; }
    public string? Address { get; set; }

    // This will become an array in JSON
    [JsonPropertyName("latlng")]
    public double[] latlng => new double[] { Latitude, Longitude };

    // Internal use
    [JsonIgnore]
    public double Latitude { get; set; }
    [JsonIgnore]
    public double Longitude { get; set; }
}
