using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class NewsRepository(AppDbContext _db) : INewsRepository
{
    public async Task<IEnumerable<News>> GetAllAsync() => await _db.News.ToListAsync();

    public IQueryable<News> Query() => _db.News.AsNoTracking();

    public async Task AddAsync(News entity)
    {
        _db.News.Add(entity);
        await _db.SaveChangesAsync();
    }

    public async Task<News?> GetByIdAsync(int id) => await _db.News.FindAsync(id);

    public async Task<News?> UpdateAsync(int id, News updated)
    {
        var existing = await _db.News.FindAsync(id);
        if (existing == null)
            return null;

        // Update all properties
        existing.Href = updated.Href;
        existing.Img = updated.Img;
        existing.Card = updated.Card;
        existing.Card2 = updated.Card2;
        existing.Title = updated.Title;
        existing.Para = updated.Para;
        existing.Views = updated.Views;
        existing.Comment = updated.Comment;
        existing.Date = updated.Date;
        existing.Video = updated.Video;
        existing.IsActive = updated.IsActive;

        await _db.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var news = await _db.News.FindAsync(id);
        if (news == null)
            return false;

        _db.News.Remove(news);
        await _db.SaveChangesAsync();
        return true;
    }
}
