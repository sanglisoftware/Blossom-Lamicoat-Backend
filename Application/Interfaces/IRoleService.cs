using Api.Application.DTOs;
namespace Api.Application.Interfaces;

public interface IRoleService
{
    Task<PagedResultDto<RoleDto>> GetAllAsync(PagedQueryDto query);
    Task<IEnumerable<RoleDto>> GetAllAsync();
    Task<RoleDto?> GetByIdAsync(int id);
    Task<RoleDto> CreateAsync(RoleDto dto);
    Task<RoleDto?> UpdateAsync(int id, RoleDto dto);
    Task<bool> DeleteAsync(int id);
}
