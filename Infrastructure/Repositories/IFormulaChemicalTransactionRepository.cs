using Api.Domain.Entities;

public interface IFormulaChemicalTransactionRepository
{
    IQueryable<FormulaChemicalTransaction> Query();
    Task<FormulaChemicalTransaction?> GetByIdAsync(int id);
    Task<FormulaChemicalTransaction> AddAsync(FormulaChemicalTransaction entity);
    Task<FormulaChemicalTransaction?> UpdateAsync(int id, FormulaChemicalTransaction entity);
    Task<bool> DeleteAsync(int id);
}
