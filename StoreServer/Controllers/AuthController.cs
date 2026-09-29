using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreServer.models;
using StoreServer.models.DTO;
using System.Security.Cryptography;
using System.Text;

namespace StoreServer.Controllers
{
    public class AuthController : Controller
    {
        private readonly WebStoreContext _context;

        public AuthController(WebStoreContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDTO dto)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Login == dto.Login);

            if (user == null)
            {
                return Unauthorized("Неверный логин или пароль");
            }


            bool passwordIsValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!passwordIsValid)
            {
                return Unauthorized("Неверный логин или пароль");
            }

            return Ok(new
            {
                idUser = user.IdUser,
                login = user.Login,
                idRole = user.IdRole
            });
        }
    }
}

