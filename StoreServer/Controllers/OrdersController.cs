using Microsoft.AspNetCore.Mvc;
using StoreServer.models;
using StoreServer.models.DTO;

namespace StoreServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly WebStoreContext _context;

        public OrdersController(WebStoreContext context)
        {
            _context = context;
        }

        private static OrderDTO ToDTO(Order order)
        {
            return new OrderDTO
            {
                IdOrder = order.IdOrder,
                IdUser = order.IdUser,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                OrderStatus = order.OrderStatus
            };
        }

        [HttpGet]
        public IActionResult GetAll(int page = 1, int pageSize = 10)
        {
            var orders = _context.Orders
                .OrderBy(o => o.IdOrder)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList()
                .Select(ToDTO)
                .ToList();

            return Ok(orders);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(ToDTO(order));
        }

        [HttpPost]
        public IActionResult Create(CreateOrderDTO dto)
        {
            var order = new Order
            {
                IdUser = dto.IdUser,
                OrderDate = dto.OrderDate,
                TotalAmount = dto.TotalAmount,
                OrderStatus = dto.OrderStatus
            };

            _context.Orders.Add(order);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = order.IdOrder }, ToDTO(order));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateOrderDTO dto)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            order.IdUser = dto.IdUser;
            order.OrderDate = dto.OrderDate;
            order.TotalAmount = dto.TotalAmount;
            order.OrderStatus = dto.OrderStatus;

            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(int id, PatchOrderDTO dto)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            if (dto.IdUser != null)
            {
                order.IdUser = dto.IdUser.Value;
            }
            if (dto.OrderDate != null)
            {
                order.OrderDate = dto.OrderDate.Value;
            }
            if (dto.TotalAmount != null)
            {
                order.TotalAmount = dto.TotalAmount.Value;
            }
            if (dto.OrderStatus != null)
            {
                order.OrderStatus = dto.OrderStatus;
            }

            _context.SaveChanges();
            return NoContent();
        }
    }
}
