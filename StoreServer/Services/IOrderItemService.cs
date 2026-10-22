using StoreServer.models.DTO;

namespace StoreServer.Services
{
    public interface IOrderItemService
    {
        Task<(List<OrderItemDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct);
        Task<OrderItemDTO?> GetByIdAsync(int id, CancellationToken ct);
        Task<OrderItemDTO> CreateAsync(CreateOrderItemDTO dto, CancellationToken ct);
        Task<OrderItemDTO?> UpdateAsync(int id, UpdateOrderItemDTO dto, CancellationToken ct);
        Task<OrderItemDTO?> PatchAsync(int id, PatchOrderItemDTO dto, CancellationToken ct);
        Task<bool> DeleteAsync(int id, CancellationToken ct);
    }
}
