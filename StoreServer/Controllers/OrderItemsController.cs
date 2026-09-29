using Microsoft.AspNetCore.Mvc;
using StoreServer.models;
using StoreServer.models.DTO;

namespace StoreServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemsController : ControllerBase
    {
        private readonly WebStoreContext _context;

        public OrderItemsController(WebStoreContext context)
        {
            _context = context;
        }

        private static OrderItemDTO ToDTO(OrderItem orderItem)
        {
            return new OrderItemDTO
            {
                IdOrderItem = orderItem.IdOrderItem,
                IdOrder = orderItem.IdOrder,
                IdProduct = orderItem.IdProduct,
                Quantity = orderItem.Quantity,
                PriceAtMoment = orderItem.PriceAtMoment
            };
        }

        [HttpGet]
        public IActionResult GetAll(int page = 1, int pageSize = 10)
        {
            var orderItems = _context.OrderItems
                .OrderBy(o => o.IdOrderItem)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList()
                .Select(ToDTO)
                .ToList();

            return Ok(orderItems);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var orderItem = _context.OrderItems.Find(id);
            if (orderItem == null)
            {
                return NotFound();
            }

            return Ok(ToDTO(orderItem));
        }

        [HttpPost]
        public IActionResult Create(CreateOrderItemDTO dto)
        {
            var orderItem = new OrderItem
            {
                IdOrder = dto.IdOrder,
                IdProduct = dto.IdProduct,
                Quantity = dto.Quantity,
                PriceAtMoment = dto.PriceAtMoment
            };

            _context.OrderItems.Add(orderItem);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = orderItem.IdOrderItem }, ToDTO(orderItem));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateOrderItemDTO dto)
        {
            var orderItem = _context.OrderItems.Find(id);
            if (orderItem == null)
            {
                return NotFound();
            }

            orderItem.IdOrder = dto.IdOrder;
            orderItem.IdProduct = dto.IdProduct;
            orderItem.Quantity = dto.Quantity;
            orderItem.PriceAtMoment = dto.PriceAtMoment;

            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var orderItem = _context.OrderItems.Find(id);
            if (orderItem == null)
            {
                return NotFound();
            }

            _context.OrderItems.Remove(orderItem);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(int id, PatchOrderItemDTO dto)
        {
            var orderItem = _context.OrderItems.Find(id);
            if (orderItem == null)
            {
                return NotFound();
            }

            if (dto.IdOrder != null)
            {
                orderItem.IdOrder = dto.IdOrder.Value;
            }
            if (dto.IdProduct != null)
            {
                orderItem.IdProduct = dto.IdProduct.Value;
            }
            if (dto.Quantity != null)
            {
                orderItem.Quantity = dto.Quantity.Value;
            }
            if (dto.PriceAtMoment != null)
            {
                orderItem.PriceAtMoment = dto.PriceAtMoment.Value;
            }

            _context.SaveChanges();
            return NoContent();
        }
    }
}
