using StoreServer.models;

namespace StoreServer.Repositories
{
    public interface ICartItemRepository
    {
        Task<List<CartItem>> GetAllAsync(int page, int pageSize, CancellationToken ct);
        Task<int> CountAsync(CancellationToken ct);
        Task<CartItem?> GetByIdAsync(int id, CancellationToken ct);
        Task<bool> CartExistsAsync(int id, CancellationToken ct);
        Task<bool> ProductExistsAsync(int id, CancellationToken ct);
        Task AddAsync(CartItem cartItem, CancellationToken ct);
        void Update(CartItem cartItem);
        void Remove(CartItem cartItem);
        Task SaveChangesAsync(CancellationToken ct);
    }
}
