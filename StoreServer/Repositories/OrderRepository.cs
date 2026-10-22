using Microsoft.EntityFrameworkCore;
using StoreServer.models;

namespace StoreServer.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly WebStoreContext _context;

        public OrderRepository(WebStoreContext context)
        {
            _context = context;
        }

        private IQueryable<Order> Query()
        {
            return _context.Orders
                .Include(x => x.User)
                .Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x.Category);
        }

        public Task<List<Order>> GetAllAsync(int page, int pageSize, CancellationToken ct)
        {
            return Query()
                .OrderBy(x => x.IdOrder)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
        }

        public Task<int> CountAsync(CancellationToken ct)
        {
            return _context.Orders.CountAsync(ct);
        }

        public async Task<Order?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await Query().FirstOrDefaultAsync(x => x.IdOrder == id, ct);
        }

        public Task<bool> UserExistsAsync(int id, CancellationToken ct)
        {
            return _context.Users.AnyAsync(x => x.IdUser == id, ct);
        }

        public async Task AddAsync(Order order, CancellationToken ct)
        {
            await _context.Orders.AddAsync(order, ct);
        }

        public void Update(Order order)
        {
            _context.Entry(order).State = EntityState.Modified;
        }

        public void Remove(Order order)
        {
            _context.Orders.Remove(order);
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
