using StoreServer.models.DTO;

namespace StoreServer.Services
{
    public interface ICartItemService
    {
        Task<(List<CartItemDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct);
        Task<CartItemDTO?> GetByIdAsync(int id, CancellationToken ct);
        Task<CartItemDTO> CreateAsync(CreateCartItemDTO dto, CancellationToken ct);
        Task<CartItemDTO?> UpdateAsync(int id, UpdateCartItemDTO dto, CancellationToken ct);
        Task<CartItemDTO?> PatchAsync(int id, PatchCartItemDTO dto, CancellationToken ct);
        Task<bool> DeleteAsync(int id, CancellationToken ct);
    }
}
