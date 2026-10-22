using StoreServer.models.DTO;

namespace StoreServer.Services
{
    public interface ICartService
    {
        Task<(List<CartDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct);
        Task<CartDTO?> GetByIdAsync(int id, CancellationToken ct);
        Task<CartDTO> CreateAsync(CreateCartDTO dto, CancellationToken ct);
        Task<CartDTO?> UpdateAsync(int id, UpdateCartDTO dto, CancellationToken ct);
        Task<CartDTO?> PatchAsync(int id, PatchCartDTO dto, CancellationToken ct);
        Task<bool> DeleteAsync(int id, CancellationToken ct);
    }
}
