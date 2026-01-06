using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace Api.Infrastructure.Repositories;

public class SliderRepository(AppDbContext _db) : ISliderRepository
{
    public async Task<IEnumerable<Slider>> GetAllAsync() => await _db.Sliders.ToListAsync();
    public async Task AddAsync(Slider slider)
    {
        _db.Sliders.Add(slider);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var slider = await _db.Sliders.FindAsync(id);
        if (slider is null) return false;

        _db.Sliders.Remove(slider);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<Slider?> GetByIdAsync(int id) => await _db.Sliders.FindAsync(id);

    public IQueryable<Slider> Query() => _db.Sliders.AsNoTracking();

    public async Task<Slider?> UpdateAsync(int id, Slider updated)
    {
        var slider = await _db.Sliders.FindAsync(id);
        if (slider is null) return null;

        slider.SequenceNo = updated.SequenceNo;
        slider.IsActive = updated.IsActive;
        slider.ImagePath = updated.ImagePath;

        await _db.SaveChangesAsync();
        return slider;
    }
}