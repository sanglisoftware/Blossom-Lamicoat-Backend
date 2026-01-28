namespace Api.Application.DTOs;

public class EmployeeDto
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string? MiddleName { get; set; }

    public string LastName { get; set; } = string.Empty;

    public string Mobile { get; set; } = string.Empty;

    public short? Type { get; set; }


    public int RoleId { get; set; }

    public string? Role { get; set; }
    public string Username { get; set; } = string.Empty;

    public short? ActiveStatus { get; set; }

    public string Password { get; set; } = string.Empty;

    public string RoleValue { get; set; } = string.Empty;
}