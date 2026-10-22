using StoreServer.models.DTO;

namespace StoreServer.Services
{
    public interface IUserService
    {
        Task<(List<UserDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct);
        Task<UserDTO?> GetByIdAsync(int id, CancellationToken ct);
        Task<UserDTO> CreateAsync(CreateUserDTO dto, CancellationToken ct);
        Task<UserDTO?> UpdateAsync(int id, UpdateUserDTO dto, CancellationToken ct);
        Task<UserDTO?> PatchAsync(int id, PatchUserDTO dto, CancellationToken ct);
        Task<bool> DeleteAsync(int id, CancellationToken ct);
    }
}
