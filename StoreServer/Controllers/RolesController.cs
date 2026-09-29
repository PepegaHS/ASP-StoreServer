using Microsoft.AspNetCore.Mvc;
using StoreServer.models;
using StoreServer.models.DTO;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StoreServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly WebStoreContext _context;

        public RolesController(WebStoreContext context)
        {
            _context = context;
        }

        private static RoleDTO ToDTO(Role role)
        {
            return new RoleDTO
            {
                IdRole = role.IdRole,
                RoleName = role.RoleName
            };
        }

        [HttpGet]
        public IActionResult GetAll(int page = 1, int pageSize = 10)
        {
            var roles = _context.Roles
                .OrderBy(p => p.IdRole)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList()
                .Select(ToDTO)
                .ToList();

            return Ok(roles);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var role = _context.Roles.Find(id);
            if (role == null)
            {
                return NotFound();
            }
            return Ok(ToDTO(role));
        }

        [HttpPost]
        public IActionResult Create(CreateRoleDTO dto)
        {
            var role = new Role
            {
                RoleName = dto.RoleName
            };
            _context.Roles.Add(role);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = role.IdRole }, ToDTO(role));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateRoleDTO dto)
        {
            var role = _context.Roles.Find(id);
            if (role == null)
            {
                return NotFound();
            }

            role.RoleName = dto.RoleName;

            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var role = _context.Roles.Find(id);
            if (role == null)
            {
                return NotFound();
            }

            _context.Roles.Remove(role);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(int id, PatchRoleDTO dto)
        {
            var role = _context.Roles.Find(id);
            if (role == null)
            {
                return NotFound();
            }

            if (dto.RoleName != null)
            {
                role.RoleName = dto.RoleName;
            }

            _context.SaveChanges();
            return NoContent();
        }
    }
}