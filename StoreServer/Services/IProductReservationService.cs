using StoreServer.models.DTO;

namespace StoreServer.Services
{
    public interface IProductReservationService
    {
        Task<(List<ProductReservationDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct);
        Task<ProductReservationDTO?> GetByIdAsync(int id, CancellationToken ct);
        Task<ProductReservationDTO> CreateAsync(CreateProductReservationDTO dto, CancellationToken ct);
        Task<ProductReservationDTO?> UpdateAsync(int id, UpdateProductReservationDTO dto, CancellationToken ct);
        Task<ProductReservationDTO?> PatchAsync(int id, PatchProductReservationDTO dto, CancellationToken ct);
        Task<bool> CancelAsync(int id, CancellationToken ct);
    }
}
