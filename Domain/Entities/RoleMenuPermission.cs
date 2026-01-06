namespace Api.Domain.Entities;

public class RoleMenuPermission
{
    public int RoleId { get; set; }
    public int MenuId { get; set; }
    public bool CreatePermission { get; set; } = false; // Default value
    public bool UpdatePermission { get; set; } = false; // Default value
    public bool DeletePermission { get; set; } = false; // Default value
}