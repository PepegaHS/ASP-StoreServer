using Microsoft.AspNetCore.Mvc;
using StoreServer.models.DTO;
using StoreServer.Services;

namespace StoreServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            try
            {
                var (items, total) = await _service.GetAllAsync(page, pageSize, cancellationToken);
                Response.Headers["X-Total-Count"] = total.ToString();
                return Ok(items);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
        {
            var item = await _service.GetByIdAsync(id, cancellationToken);
            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var item = await _service.CreateAsync(dto, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = item.IdUser }, item);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateUserDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var item = await _service.UpdateAsync(id, dto, cancellationToken);
                if (item == null)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, PatchUserDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var item = await _service.PatchAsync(id, dto, cancellationToken);
                if (item == null)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var deleted = await _service.DeleteAsync(id, cancellationToken);
                if (!deleted)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
