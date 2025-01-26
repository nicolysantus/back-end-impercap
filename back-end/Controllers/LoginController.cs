using back_end.Models;
using back_end.Services;
using Microsoft.AspNetCore.Mvc;

namespace back_end.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly AuthService _authService;

        public LoginController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserModel user)
        {
            var token = _authService.Authenticate(user.Username, user.Password);
            if (token == null)
            {
                return Unauthorized("Credenciais inválidas.");
            }
            return Ok(new { Token = token });
        }
    }
}
