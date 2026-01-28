using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Domain.Entities;

public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public short? Type { get; set; }

    public int RoleId { get; set; }

    public Role? Role { get; set; }
    public string Username { get; set; } = string.Empty;
    public short? ActiveStatus { get; set; }

    [NotMapped]
    public string? RoleValue { get; set; }
}
