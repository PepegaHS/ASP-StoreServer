using Microsoft.EntityFrameworkCore;
using StoreServer.models;

namespace StoreServer.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly WebStoreContext _context;

        public RoleRepository(WebStoreContext context)
        {
            _context = context;
        }

        private IQueryable<Role> Query()
        {
            return _context.Roles
                .Include(x => x.Users);
        }

        public Task<List<Role>> GetAllAsync(int page, int pageSize, CancellationToken ct)
        {
            return Query()
                .OrderBy(x => x.IdRole)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
        }

        public Task<int> CountAsync(CancellationToken ct)
        {
            return _context.Roles.CountAsync(ct);
        }

        public async Task<Role?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await Query().FirstOrDefaultAsync(x => x.IdRole == id, ct);
        }

        public async Task AddAsync(Role role, CancellationToken ct)
        {
            await _context.Roles.AddAsync(role, ct);
        }

        public void Update(Role role)
        {
            _context.Entry(role).State = EntityState.Modified;
        }

        public void Remove(Role role)
        {
            _context.Roles.Remove(role);
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
