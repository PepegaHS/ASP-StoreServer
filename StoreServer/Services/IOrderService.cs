using StoreServer.models.DTO;

namespace StoreServer.Services
{
    public interface IOrderService
    {
        Task<(List<OrderDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct);
        Task<OrderDTO?> GetByIdAsync(int id, CancellationToken ct);
        Task<OrderDTO> CreateAsync(CreateOrderDTO dto, CancellationToken ct);
        Task<OrderDTO?> UpdateAsync(int id, UpdateOrderDTO dto, CancellationToken ct);
        Task<OrderDTO?> PatchAsync(int id, PatchOrderDTO dto, CancellationToken ct);
        Task<bool> DeleteAsync(int id, CancellationToken ct);
    }
}
