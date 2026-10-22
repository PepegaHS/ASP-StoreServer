using StoreServer.models;

namespace StoreServer.Repositories
{
    public interface IOrderItemRepository
    {
        Task<List<OrderItem>> GetAllAsync(int page, int pageSize, CancellationToken ct);
        Task<int> CountAsync(CancellationToken ct);
        Task<OrderItem?> GetByIdAsync(int id, CancellationToken ct);
        Task<bool> OrderExistsAsync(int id, CancellationToken ct);
        Task<bool> ProductExistsAsync(int id, CancellationToken ct);
        Task AddAsync(OrderItem orderItem, CancellationToken ct);
        void Update(OrderItem orderItem);
        void Remove(OrderItem orderItem);
        Task SaveChangesAsync(CancellationToken ct);
    }
}
