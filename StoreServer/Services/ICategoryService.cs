using StoreServer.models.DTO;

namespace StoreServer.Services
{
    public interface ICategoryService
    {
        Task<(List<CategoryDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct);
        Task<CategoryDTO?> GetByIdAsync(int id, CancellationToken ct);
        Task<CategoryDTO> CreateAsync(CreateCategoryDTO dto, CancellationToken ct);
        Task<CategoryDTO?> UpdateAsync(int id, UpdateCategoryDTO dto, CancellationToken ct);
        Task<CategoryDTO?> PatchAsync(int id, PatchCategoryDTO dto, CancellationToken ct);
        Task<bool> DeleteAsync(int id, CancellationToken ct);
    }
}
