// Application/DTOs/MenuDto.cs
namespace Api.Application.DTOs;

public class MenuDto
{
    public int Id { get; set; }
    public string Icon { get; set; } = string.Empty;
    public string? Pathname { get; set; }
    public string Title { get; set; } = string.Empty;
    public PermissionDto? Permission { get; set; }
    public List<MenuDto>? SubMenu { get; set; }
}

// Application/DTOs/MenuDto.cs
public class PermissionDto
{
    public bool Create { get; set; }
    public bool Update { get; set; }
    public bool Delete { get; set; }

    // Add parameterless constructor
    public PermissionDto() { }

    public PermissionDto(bool create, bool update, bool delete)
    {
        Create = create;
        Update = update;
        Delete = delete;
    }
}

//to save permission
public class MenuPermissionDto
{
    public int MenuId { get; set; }
    public bool Create { get; set; }
    public bool Update { get; set; }
    public bool Delete { get; set; }
}

public class RoleMenuPermissionRequest
{
    public int RoleId { get; set; }
    public List<MenuPermissionDto> Permissions { get; set; } = new();
}

//below is for admin panel
public class MenuSelectionDto
{
    public int Id { get; set; }
    public string Icon { get; set; } = string.Empty;
    public string? Pathname { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool Selected { get; set; } 
     public PermissionDto? Permission { get; set; } 
    public List<MenuSelectionDto>? SubMenu { get; set; }
    
}