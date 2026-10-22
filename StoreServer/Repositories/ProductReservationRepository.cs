using Microsoft.EntityFrameworkCore;
using StoreServer.models;

namespace StoreServer.Repositories
{
    public class ProductReservationRepository : IProductReservationRepository
    {
        private readonly WebStoreContext _context;

        public ProductReservationRepository(WebStoreContext context)
        {
            _context = context;
        }

        private IQueryable<ProductReservation> Query()
        {
            return _context.ProductReservations
                .Include(x => x.Product).ThenInclude(x => x.Category)
                .Include(x => x.User);
        }

        public Task<List<ProductReservation>> GetAllAsync(int page, int pageSize, CancellationToken ct)
        {
            return Query().OrderBy(x => x.IdProductReservation)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        }

        public Task<int> CountAsync(CancellationToken ct)
        {
            return _context.ProductReservations.CountAsync(ct);
        }

        public Task<ProductReservation?> GetByIdAsync(int id, CancellationToken ct)
        {
            return Query().FirstOrDefaultAsync(x => x.IdProductReservation == id, ct);
        }

        public Task<Product?> FindProductAsync(int id, CancellationToken ct)
        {
            return _context.Products.Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.IdProduct == id, ct);
        }

        public Task<bool> UserExistsAsync(int id, CancellationToken ct)
        {
            return _context.Users.AnyAsync(x => x.IdUser == id, ct);
        }

        public async Task AddAsync(ProductReservation reservation, CancellationToken ct)
        {
            await _context.ProductReservations.AddAsync(reservation, ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct)
        {
            try
            {
                // EF saves the tracked product and reservation atomically in one call.
                await _context.SaveChangesAsync(ct);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new InvalidOperationException("Остаток или резерв изменился. Повторите запрос.", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Не удалось сохранить резерв: проверьте связанные записи.", ex);
            }
        }
    }
}
