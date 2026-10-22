using Microsoft.AspNetCore.Mvc;
using StoreServer.models.DTO;
using StoreServer.Services;

namespace StoreServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
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
        public async Task<IActionResult> GetCategoryById(int id, CancellationToken cancellationToken = default)
        {
            var item = await _service.GetByIdAsync(id, cancellationToken);
            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var item = await _service.CreateAsync(dto, cancellationToken);
                return CreatedAtAction(nameof(GetCategoryById), new { id = item.IdCategory }, item);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDTO dto, CancellationToken cancellationToken = default)
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
        public async Task<IActionResult> PatchCategory(int id, PatchCategoryDTO dto, CancellationToken cancellationToken = default)
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
        public async Task<IActionResult> DeleteCategory(int id, CancellationToken cancellationToken = default)
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
