using StoreServer.models;

namespace StoreServer.Repositories
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllAsync(int page, int pageSize, CancellationToken ct);
        Task<int> CountAsync(CancellationToken ct);
        Task<Order?> GetByIdAsync(int id, CancellationToken ct);
        Task<bool> UserExistsAsync(int id, CancellationToken ct);
        Task AddAsync(Order order, CancellationToken ct);
        void Update(Order order);
        void Remove(Order order);
        Task SaveChangesAsync(CancellationToken ct);
    }
}
