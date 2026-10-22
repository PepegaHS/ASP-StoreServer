using StoreServer.models.DTO;

namespace StoreServer.Services
{
    public interface IProductService
    {
        Task<(List<ProductDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct);
        Task<ProductDTO?> GetByIdAsync(int id, CancellationToken ct);
        Task<ProductDTO> CreateAsync(CreateProductDTO dto, CancellationToken ct);
        Task<ProductDTO?> UpdateAsync(int id, UpdateProductDTO dto, CancellationToken ct);
        Task<ProductDTO?> PatchAsync(int id, PatchProductDTO dto, CancellationToken ct);
        Task<bool> DeleteAsync(int id, CancellationToken ct);
    }
}
