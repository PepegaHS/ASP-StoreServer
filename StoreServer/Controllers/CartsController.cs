using Microsoft.AspNetCore.Mvc;
using StoreServer.models;
using StoreServer.models.DTO;

namespace StoreServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly WebStoreContext _context;

        public CartsController(WebStoreContext context)
        {
            _context = context;
        }

        private static CartDTO ToDTO(Cart cart)
        {
            return new CartDTO
            {
                IdCart = cart.IdCart,
                IdUser = cart.IdUser,
                CreatedAt = cart.CreatedAt
            };
        }

        [HttpGet]
        public IActionResult GetAll(int page = 1, int pageSize = 10)
        {
            var carts = _context.Carts
                .OrderBy(c => c.IdCart)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList()
                .Select(ToDTO)
                .ToList();

            return Ok(carts);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var cart = _context.Carts.Find(id);
            if (cart == null)
            {
                return NotFound();
            }

            return Ok(ToDTO(cart));
        }

        [HttpPost]
        public IActionResult Create(CreateCartDTO dto)
        {
            var cart = new Cart
            {
                IdUser = dto.IdUser,
                CreatedAt = dto.CreatedAt
            };

            _context.Carts.Add(cart);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = cart.IdCart }, ToDTO(cart));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateCartDTO dto)
        {
            var cart = _context.Carts.Find(id);
            if (cart == null)
            {
                return NotFound();
            }

            cart.IdUser = dto.IdUser;
            cart.CreatedAt = dto.CreatedAt;

            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var cart = _context.Carts.Find(id);
            if (cart == null)
            {
                return NotFound();
            }

            _context.Carts.Remove(cart);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(int id, PatchCartDTO dto)
        {
            var cart = _context.Carts.Find(id);
            if (cart == null)
            {
                return NotFound();
            }

            if (dto.IdUser != null)
            {
                cart.IdUser = dto.IdUser.Value;
            }
            if (dto.CreatedAt != null)
            {
                cart.CreatedAt = dto.CreatedAt.Value;
            }

            _context.SaveChanges();
            return NoContent();
        }
    }
}
