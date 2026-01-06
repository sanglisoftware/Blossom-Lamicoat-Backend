namespace Api.Domain.Entities;

public class User
{
    public required string Username { get; set; }
    public required string Password { get; set; } 
    public int Role { get; set; }
    public bool LoginStatus { get; set; }
    public DateTime? LastLoginDate { get; set; }
}
