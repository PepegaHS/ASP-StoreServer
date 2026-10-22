using StoreServer.models.DTO;

namespace StoreServer.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDTO?> LoginAsync(LoginDTO dto, CancellationToken ct);
    }
}
