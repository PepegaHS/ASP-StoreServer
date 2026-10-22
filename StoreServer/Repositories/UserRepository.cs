using Microsoft.EntityFrameworkCore;
using StoreServer.models;

namespace StoreServer.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly WebStoreContext _context;

        public UserRepository(WebStoreContext context)
        {
            _context = context;
        }

        private IQueryable<User> Query()
        {
            return _context.Users
                .Include(x => x.Role);
        }

        public Task<List<User>> GetAllAsync(int page, int pageSize, CancellationToken ct)
        {
            return Query()
                .OrderBy(x => x.IdUser)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
        }

        public Task<int> CountAsync(CancellationToken ct)
        {
            return _context.Users.CountAsync(ct);
        }

        public async Task<User?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await Query().FirstOrDefaultAsync(x => x.IdUser == id, ct);
        }

        public Task<bool> RoleExistsAsync(int id, CancellationToken ct)
        {
            return _context.Roles.AnyAsync(x => x.IdRole == id, ct);
        }

        public Task<User?> FindByLoginAsync(string login, CancellationToken ct)
        {
            return _context.Users.FirstOrDefaultAsync(user => user.Login == login, ct);
        }

        public async Task AddAsync(User user, CancellationToken ct)
        {
            await _context.Users.AddAsync(user, ct);
        }

        public void Update(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }

        public void Remove(User user)
        {
            _context.Users.Remove(user);
        }

        public async Task SaveChangesAsync(CancellationToken ct)
        {
            try
            {
                await _context.SaveChangesAsync(ct);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new InvalidOperationException("Данные изменились. Повторите запрос.", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Изменение нарушает ограничения базы данных (связи или уникальность).", ex);
            }
        }
    }
}
