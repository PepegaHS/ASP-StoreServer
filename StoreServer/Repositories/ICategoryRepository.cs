using StoreServer.models;

namespace StoreServer.Repositories
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync(int page, int pageSize, CancellationToken ct);
        Task<int> CountAsync(CancellationToken ct);
        Task<Category?> GetByIdAsync(int id, CancellationToken ct);
        Task AddAsync(Category category, CancellationToken ct);
        void Update(Category category);
        void Remove(Category category);
        Task SaveChangesAsync(CancellationToken ct);
    }
}
