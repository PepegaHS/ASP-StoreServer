using Microsoft.AspNetCore.Mvc;
using StoreServer.models;
using StoreServer.models.DTO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StoreServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly WebStoreContext _context;

        public CategoriesController(WebStoreContext context)
        {
            _context = context;
        }

        private static Category ToDTO(Category category)
        {
            return new Category
            {
                IdCategory = category.IdCategory,
                CategoryName = category.CategoryName,
                CategoryDescription = category.CategoryDescription
            };
        }

        [HttpGet]
        public IActionResult GetAllCategories(int page = 1, int pageSize = 10)
        {
            var categories = _context.Categories
                .OrderBy(c => c.IdCategory)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList()
                .Select(ToDTO)
                .ToList();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public IActionResult GetCategoryById(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(ToDTO(category));
        }

        [HttpPost]
        public IActionResult CreateCategory(CreateCategoryDTO dto)
        {
            var category = new Category
            {
                CategoryName = dto.CategoryName,
                CategoryDescription = dto.CategoryDescription
            };
            _context.Categories.Add(category);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetCategoryById), new { id = category.IdCategory }, ToDTO(category));
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCategory(int id, UpdateCategoryDTO dto)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            category.CategoryName = dto.CategoryName;
            category.CategoryDescription = dto.CategoryDescription;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PatchCategory(int id, PatchCategoryDTO dto)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            if (dto.CategoryName != null)
            {
                category.CategoryName = dto.CategoryName;
            }
            if (dto.CategoryDescription != null)
            {
                category.CategoryDescription = dto.CategoryDescription;
            }
            _context.SaveChanges();
            return NoContent();
        }

    }
}
