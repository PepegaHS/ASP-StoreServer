using Microsoft.EntityFrameworkCore;
using StoreServer.models;

namespace StoreServer.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly WebStoreContext _context;

        public OrderItemRepository(WebStoreContext context)
        {
            _context = context;
        }

        private IQueryable<OrderItem> Query()
        {
            return _context.OrderItems
                .Include(x => x.Order)
                .Include(x => x.Product).ThenInclude(x => x.Category);
        }

        public Task<List<OrderItem>> GetAllAsync(int page, int pageSize, CancellationToken ct)
        {
            return Query()
                .OrderBy(x => x.IdOrderItem)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
        }

        public Task<int> CountAsync(CancellationToken ct)
        {
            return _context.OrderItems.CountAsync(ct);
        }

        public async Task<OrderItem?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await Query().FirstOrDefaultAsync(x => x.IdOrderItem == id, ct);
        }

        public Task<bool> OrderExistsAsync(int id, CancellationToken ct)
        {
            return _context.Orders.AnyAsync(x => x.IdOrder == id, ct);
        }

        public Task<bool> ProductExistsAsync(int id, CancellationToken ct)
        {
            return _context.Products.AnyAsync(x => x.IdProduct == id, ct);
        }

        public async Task AddAsync(OrderItem orderItem, CancellationToken ct)
        {
            await _context.OrderItems.AddAsync(orderItem, ct);
        }

        public void Update(OrderItem orderItem)
        {
            _context.Entry(orderItem).State = EntityState.Modified;
        }

        public void Remove(OrderItem orderItem)
        {
            _context.OrderItems.Remove(orderItem);
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
