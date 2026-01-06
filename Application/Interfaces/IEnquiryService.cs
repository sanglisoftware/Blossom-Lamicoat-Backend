using Api.Application.DTOs;

namespace Api.Application.Interfaces;

public interface IEnquiryService
{
    Task<PagedResultDto<EnquiryResponseDto>> GetAllAsync(PagedQueryDto query);
    Task<EnquiryResponseDto?> GetByIdAsync(int id);
    Task<EnquiryResponseDto> CreateAsync(EnquiryResponseDto dto);
    Task<EnquiryResponseDto?> UpdateAsync(int id, EnquiryResponseDto dto);
    Task<bool> DeleteAsync(int id);
    Task<EnquiryResponseDto?> UpdateStatusAsync(int id, short isActive);
}

