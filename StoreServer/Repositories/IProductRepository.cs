using StoreServer.models;

namespace StoreServer.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync(int page, int pageSize, CancellationToken ct);
        Task<int> CountAsync(CancellationToken ct);
        Task<Product?> GetByIdAsync(int id, CancellationToken ct);
        Task<bool> CategoryExistsAsync(int id, CancellationToken ct);
        Task AddAsync(Product product, CancellationToken ct);
        void Update(Product product);
        void Remove(Product product);
        Task SaveChangesAsync(CancellationToken ct);
    }
}
