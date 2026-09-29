using Microsoft.AspNetCore.Mvc;
using StoreServer.models;
using StoreServer.models.DTO;

namespace StoreServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemsController : ControllerBase
    {
        private readonly WebStoreContext _context;

        public CartItemsController(WebStoreContext context)
        {
            _context = context;
        }

        private static CartItemDTO ToDTO(CartItem cartItem)
        {
            return new CartItemDTO
            {
                IdCartItem = cartItem.IdCartItem,
                IdCart = cartItem.IdCart,
                IdProduct = cartItem.IdProduct,
                Quantity = cartItem.Quantity
            };
        }

        [HttpGet]
        public IActionResult GetAll(int page = 1, int pageSize = 10)
        {
            var cartItems = _context.CartItems
                .OrderBy(c => c.IdCartItem)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList()
                .Select(ToDTO)
                .ToList();

            return Ok(cartItems);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var cartItem = _context.CartItems.Find(id);
            if (cartItem == null)
            {
                return NotFound();
            }

            return Ok(ToDTO(cartItem));
        }

        [HttpPost]
        public IActionResult Create(CreateCartItemDTO dto)
        {
            var cartItem = new CartItem
            {
                IdCart = dto.IdCart,
                IdProduct = dto.IdProduct,
                Quantity = dto.Quantity
            };

            _context.CartItems.Add(cartItem);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = cartItem.IdCartItem }, ToDTO(cartItem));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateCartItemDTO dto)
        {
            var cartItem = _context.CartItems.Find(id);
            if (cartItem == null)
            {
                return NotFound();
            }

            cartItem.IdCart = dto.IdCart;
            cartItem.IdProduct = dto.IdProduct;
            cartItem.Quantity = dto.Quantity;

            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var cartItem = _context.CartItems.Find(id);
            if (cartItem == null)
            {
                return NotFound();
            }

            _context.CartItems.Remove(cartItem);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(int id, PatchCartItemDTO dto)
        {
            var cartItem = _context.CartItems.Find(id);
            if (cartItem == null)
            {
                return NotFound();
            }

            if (dto.IdCart != null)
            {
                cartItem.IdCart = dto.IdCart.Value;
            }
            if (dto.IdProduct != null)
            {
                cartItem.IdProduct = dto.IdProduct.Value;
            }
            if (dto.Quantity != null)
            {
                cartItem.Quantity = dto.Quantity.Value;
            }

            _context.SaveChanges();
            return NoContent();
        }
    }
}
