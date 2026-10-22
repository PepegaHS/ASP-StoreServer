using StoreServer.models;

namespace StoreServer.Repositories
{
    public interface ICartRepository
    {
        Task<List<Cart>> GetAllAsync(int page, int pageSize, CancellationToken ct);
        Task<int> CountAsync(CancellationToken ct);
        Task<Cart?> GetByIdAsync(int id, CancellationToken ct);
        Task<bool> UserExistsAsync(int id, CancellationToken ct);
        Task AddAsync(Cart cart, CancellationToken ct);
        void Update(Cart cart);
        void Remove(Cart cart);
        Task SaveChangesAsync(CancellationToken ct);
    }
}
