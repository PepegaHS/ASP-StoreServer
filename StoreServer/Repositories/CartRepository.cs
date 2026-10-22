using Microsoft.EntityFrameworkCore;
using StoreServer.models;

namespace StoreServer.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly WebStoreContext _context;

        public CartRepository(WebStoreContext context)
        {
            _context = context;
        }

        private IQueryable<Cart> Query()
        {
            return _context.Carts
                .Include(x => x.User)
                .Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x.Category);
        }

        public Task<List<Cart>> GetAllAsync(int page, int pageSize, CancellationToken ct)
        {
            return Query()
                .OrderBy(x => x.IdCart)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
        }

        public Task<int> CountAsync(CancellationToken ct)
        {
            return _context.Carts.CountAsync(ct);
        }

        public async Task<Cart?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await Query().FirstOrDefaultAsync(x => x.IdCart == id, ct);
        }

        public Task<bool> UserExistsAsync(int id, CancellationToken ct)
        {
            return _context.Users.AnyAsync(x => x.IdUser == id, ct);
        }

        public async Task AddAsync(Cart cart, CancellationToken ct)
        {
            await _context.Carts.AddAsync(cart, ct);
        }

        public void Update(Cart cart)
        {
            _context.Entry(cart).State = EntityState.Modified;
        }

        public void Remove(Cart cart)
        {
            _context.Carts.Remove(cart);
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
