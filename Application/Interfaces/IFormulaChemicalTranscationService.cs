using Api.Application.DTOs;

namespace Api.Application.Interfaces;

public interface IFormulaChemicalTransactionService
{
     Task<PagedResultDto<FormulaChemicalTransactionDto>> GetAllAsync(PagedQueryDto query);

     Task<FormulaTransactionDetailsDto?> GetByIdAsync(int id);

    Task<FormulaChemicalTransactionDto> CreateAsync(FormulaChemicalTransactionDto dto);

    Task<FormulaChemicalTransactionDto?> UpdateAsync(int id, FormulaChemicalTransactionDto dto);

    Task<bool> DeleteAsync(int id);

    Task<FormulaDetailsDto?> GetFormulaDetailsAsync(int formulaMasterId);

    Task<FormulaDetailsDto?> UpdateFormulaAsync(FormulaDetailsDto dto);

}
