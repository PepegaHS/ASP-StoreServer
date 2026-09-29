using Microsoft.AspNetCore.Mvc;
using StoreServer.models;
using StoreServer.models.DTO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StoreServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly WebStoreContext _context;

        public ProductsController(WebStoreContext context)
        {
            _context = context; 
        }

        private static Product ToDTO(Product product)
        {
            return new Product
            {
                IdProduct = product.IdProduct,
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                Price = product.Price,
                IdCategory = product.IdCategory
            };
        }

        [HttpGet]
        public IActionResult GetAll(int page = 1, int pageSize = 10)
        {
            var products = _context.Products
                .OrderBy(p => p.IdProduct)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList()
                .Select(ToDTO)
                .ToList();

            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(ToDTO(product));
        }

        [HttpPost]
        public IActionResult Create(CreateProductDTO dto)
        {
            var product = new Product
            {
                ProductName = dto.ProductName,
                ProductDescription = dto.ProductDescription,
                Price = dto.Price,
                IdCategory = dto.IdCategory
            };
            _context.Products.Add(product);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = product.IdProduct }, ToDTO(product));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateProductDTO dto)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            product.ProductName = dto.ProductName;
            product.ProductDescription = dto.ProductDescription;
            product.Price = dto.Price;
            product.IdCategory = dto.IdCategory;

            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(int id, PatchProductDTO dto)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            if (dto.ProductName != null)
            {
                product.ProductName = dto.ProductName;
            }
            if (dto.ProductDescription != null)
            {
                product.ProductDescription = dto.ProductDescription;
            }
            if (dto.Price != null)
            {
                product.Price = dto.Price.Value;
            }
            if (dto.IdCategory != null)
            {
                product.IdCategory = dto.IdCategory.Value;
            }

            _context.SaveChanges();
            return NoContent();
        }
    }
}
