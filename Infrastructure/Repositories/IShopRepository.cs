public interface IShopRepository
{
    Task AddAsync(Shop shop);
    Task<List<Shop>> GetAllAsync();

    Task<IEnumerable<Shop>> GetAllShopsForWeb();

    IQueryable<Shop> Query();
    Task<Shop?> GetByIdAsync(int id);
    Task UpdateAsync(Shop shop);
    Task DeleteAsync(Shop shop);
}
