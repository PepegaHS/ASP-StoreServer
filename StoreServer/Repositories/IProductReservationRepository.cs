using StoreServer.models;

namespace StoreServer.Repositories
{
    public interface IProductReservationRepository
    {
        Task<List<ProductReservation>> GetAllAsync(int page, int pageSize, CancellationToken ct);
        Task<int> CountAsync(CancellationToken ct);
        Task<ProductReservation?> GetByIdAsync(int id, CancellationToken ct);
        Task<Product?> FindProductAsync(int id, CancellationToken ct);
        Task<bool> UserExistsAsync(int id, CancellationToken ct);
        Task AddAsync(ProductReservation reservation, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
    }
}
