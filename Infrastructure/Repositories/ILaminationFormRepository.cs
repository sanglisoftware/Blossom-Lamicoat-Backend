using Api.Domain.Entities;

namespace Api.Infrastructure.Repositories;

public interface ILaminationFormRepository
{
    IQueryable<LaminationForm> Query();
    Task<LaminationForm?> GetByIdAsync(int id);
    Task<LaminationForm> AddAsync(LaminationForm laminationForm);
}
