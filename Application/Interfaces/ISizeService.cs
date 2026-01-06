using Api.Application.DTOs;
namespace Api.Application.Interfaces;

public interface ISizeService
{
    Task<IEnumerable<SizeDto>> GetAllAsync();
    Task<SizeDto?> GetByIdAsync(int id);
    Task<SizeDto> CreateAsync(SizeDto dto);
    Task<SizeDto?> UpdateAsync(int id, SizeDto dto);
    Task<bool> DeleteAsync(int id);
}
