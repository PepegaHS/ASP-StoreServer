using Microsoft.EntityFrameworkCore;
using StoreServer.models;

namespace StoreServer.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly WebStoreContext _context;

        public CategoryRepository(WebStoreContext context)
        {
            _context = context;
        }

        private IQueryable<Category> Query()
        {
            return _context.Categories
                .Include(x => x.Products);
        }

        public Task<List<Category>> GetAllAsync(int page, int pageSize, CancellationToken ct)
        {
            return Query()
                .OrderBy(x => x.IdCategory)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
        }

        public Task<int> CountAsync(CancellationToken ct)
        {
            return _context.Categories.CountAsync(ct);
        }

        public async Task<Category?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await Query().FirstOrDefaultAsync(x => x.IdCategory == id, ct);
        }

        public async Task AddAsync(Category category, CancellationToken ct)
        {
            await _context.Categories.AddAsync(category, ct);
        }

        public void Update(Category category)
        {
            _context.Entry(category).State = EntityState.Modified;
        }

        public void Remove(Category category)
        {
            _context.Categories.Remove(category);
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
