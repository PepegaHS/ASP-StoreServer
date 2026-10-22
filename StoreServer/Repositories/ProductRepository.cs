using Microsoft.EntityFrameworkCore;
using StoreServer.models;

namespace StoreServer.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly WebStoreContext _context;

        public ProductRepository(WebStoreContext context)
        {
            _context = context;
        }

        private IQueryable<Product> Query()
        {
            return _context.Products
                .Include(x => x.Category);
        }

        public Task<List<Product>> GetAllAsync(int page, int pageSize, CancellationToken ct)
        {
            return Query()
                .OrderBy(x => x.IdProduct)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
        }

        public Task<int> CountAsync(CancellationToken ct)
        {
            return _context.Products.CountAsync(ct);
        }

        public async Task<Product?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await Query().FirstOrDefaultAsync(x => x.IdProduct == id, ct);
        }

        public Task<bool> CategoryExistsAsync(int id, CancellationToken ct)
        {
            return _context.Categories.AnyAsync(x => x.IdCategory == id, ct);
        }

        public async Task AddAsync(Product product, CancellationToken ct)
        {
            await _context.Products.AddAsync(product, ct);
        }

        public void Update(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;
        }

        public void Remove(Product product)
        {
            _context.Products.Remove(product);
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
