using Api.Application.DTOs;
using Api.Application.Interfaces;
using AutoMapper;
using Api.Domain.Entities;
using Api.Infrastructure.Repositories;
namespace Api.Application.Services;

public class SizeService : ISizeService
{
    private readonly ISizeRepository _repository;
    private readonly IMapper _mapper;

    public SizeService(ISizeRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SizeDto>> GetAllAsync() =>
        _mapper.Map<IEnumerable<SizeDto>>(await _repository.GetAllAsync());

    public async Task<SizeDto?> GetByIdAsync(int id)
    {
        var size = await _repository.GetByIdAsync(id);
        return size is null ? null : _mapper.Map<SizeDto>(size);
    }

    public async Task<SizeDto> CreateAsync(SizeDto dto)
    {
        var size = _mapper.Map<Size>(dto);
        var created = await _repository.CreateAsync(size);
        return _mapper.Map<SizeDto>(created);
    }

    public async Task<SizeDto?> UpdateAsync(int id, SizeDto dto)
    {
        var updated = await _repository.UpdateAsync(id, _mapper.Map<Size>(dto));
        return updated is null ? null : _mapper.Map<SizeDto>(updated);
    }

    public async Task<bool> DeleteAsync(int id) =>
        await _repository.DeleteAsync(id);
}
