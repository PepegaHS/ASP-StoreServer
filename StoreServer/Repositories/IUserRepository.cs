using StoreServer.models;

namespace StoreServer.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync(int page, int pageSize, CancellationToken ct);
        Task<int> CountAsync(CancellationToken ct);
        Task<User?> GetByIdAsync(int id, CancellationToken ct);
        Task<bool> RoleExistsAsync(int id, CancellationToken ct);
        Task<User?> FindByLoginAsync(string login, CancellationToken ct);
        Task AddAsync(User user, CancellationToken ct);
        void Update(User user);
        void Remove(User user);
        Task SaveChangesAsync(CancellationToken ct);
    }
}
