namespace Api.Application.DTOs;

public class AuthRequestDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}
