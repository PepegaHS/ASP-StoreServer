using Microsoft.EntityFrameworkCore;
using StoreServer.models;

namespace StoreServer.Repositories
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly WebStoreContext _context;

        public CartItemRepository(WebStoreContext context)
        {
            _context = context;
        }

        private IQueryable<CartItem> Query()
        {
            return _context.CartItems
                .Include(x => x.Cart)
                .Include(x => x.Product).ThenInclude(x => x.Category);
        }

        public Task<List<CartItem>> GetAllAsync(int page, int pageSize, CancellationToken ct)
        {
            return Query()
                .OrderBy(x => x.IdCartItem)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
        }

        public Task<int> CountAsync(CancellationToken ct)
        {
            return _context.CartItems.CountAsync(ct);
        }

        public async Task<CartItem?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await Query().FirstOrDefaultAsync(x => x.IdCartItem == id, ct);
        }

        public Task<bool> CartExistsAsync(int id, CancellationToken ct)
        {
            return _context.Carts.AnyAsync(x => x.IdCart == id, ct);
        }

        public Task<bool> ProductExistsAsync(int id, CancellationToken ct)
        {
            return _context.Products.AnyAsync(x => x.IdProduct == id, ct);
        }

        public async Task AddAsync(CartItem cartItem, CancellationToken ct)
        {
            await _context.CartItems.AddAsync(cartItem, ct);
        }

        public void Update(CartItem cartItem)
        {
            _context.Entry(cartItem).State = EntityState.Modified;
        }

        public void Remove(CartItem cartItem)
        {
            _context.CartItems.Remove(cartItem);
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
