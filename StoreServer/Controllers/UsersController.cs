using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreServer.models;
using StoreServer.models.DTO;


namespace StoreServer.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly WebStoreContext _context;

        public UsersController(WebStoreContext context)
        {
            _context = context;
        }

        private static UserDTO ToDTO(User user)
        {
            return new UserDTO
            {
                IdUser = user.IdUser,
                Login = user.Login,
                PasswordHash = user.PasswordHash,
                IdRole = user.IdRole
            };
        }

        [HttpGet]
        public IActionResult GetAll(int page = 1, int pageSize = 10)
        {
            var users = _context.Users
                .OrderBy(u => u.IdUser)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList()
                .Select(ToDTO)
                .ToList();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(ToDTO(user));
        }

        [HttpPost]
        public IActionResult Create(CreateUserDTO dto)
        {
            var user = new User
            {
                Login = dto.Login,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                IdRole = dto.IdRole
            };
            _context.Users.Add(user);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = user.IdUser }, ToDTO(user));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateUserDTO dto)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            user.Login = dto.Login;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.IdRole = dto.IdRole;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            _context.Users.Remove(user);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(int id, PatchUserDTO dto)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            if (dto.Login != null)
            {
                user.Login = dto.Login;
            }
            if (dto.Password != null)
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }
            if (dto.IdRole.HasValue)
            {
                user.IdRole = dto.IdRole.Value;
            }
            _context.SaveChanges();
            return NoContent();
        }

    }
}
