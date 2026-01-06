namespace Api.Application.DTOs;

public class AuthResponseDto
{
    public required string Token { get; set; }
    public DateTime Expiration { get; set; }
    public string MenuJson { get; set; } = ""; 
}
