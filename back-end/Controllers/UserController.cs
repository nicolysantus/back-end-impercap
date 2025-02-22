using back_end.Data;
using back_end.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace back_end.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserModel>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserModel>> GetUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new
                {
                    status = 404,
                    message = "Usuário não encontrado.",
                    traceId = HttpContext.TraceIdentifier
                });
            }

            return Ok(user);
        }

        [HttpPost("register")]
        public async Task<ActionResult<bool>> CreateUser(UserModel user)
        {
            // Verifica se já existe um usuário com o mesmo Username ou CPF
            if (_context.Users.Any(u => u.Username == user.Username || u.CPF == user.CPF))
            {
                return Conflict(new
                {
                    status = 409,
                    message = "Já existe um usuário com este nome de usuário ou CPF.",
                    traceId = HttpContext.TraceIdentifier
                });
            }

            // Garante que o ID seja gerado corretamente
            user.Id = Guid.NewGuid();

            // Criptografa a senha
            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            }

            // Adiciona o usuário ao banco
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                status = 200,
                message = "Usuário registrado com sucesso.",
                traceId = HttpContext.TraceIdentifier
            });
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserModel user)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                return NotFound(new
                {
                    status = 404,
                    message = "Usuário não encontrado.",
                    traceId = HttpContext.TraceIdentifier
                });
            }

            if (!string.IsNullOrWhiteSpace(user.Username))
            {
                existingUser.Username = user.Username;
            }

            existingUser.UserType = user.UserType; // Booleano, sempre atualizado

            if (!string.IsNullOrWhiteSpace(user.FirstName))
            {
                existingUser.FirstName = user.FirstName;
            }

            if (!string.IsNullOrWhiteSpace(user.LastName))
            {
                existingUser.LastName = user.LastName;
            }

            if (!string.IsNullOrWhiteSpace(user.CPF))
            {
                existingUser.CPF = user.CPF;
            }

            if (!string.IsNullOrWhiteSpace(user.DateOfBirth))
            {
                existingUser.DateOfBirth = user.DateOfBirth;
            }

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                existingUser.Email = user.Email;
            }

            if (!string.IsNullOrWhiteSpace(user.Address))
            {
                existingUser.Address = user.Address;
            }

            if (!string.IsNullOrWhiteSpace(user.Number))
            {
                existingUser.Number = user.Number;
            }

            if (!string.IsNullOrWhiteSpace(user.Neighborhood))
            {
                existingUser.Neighborhood = user.Neighborhood;
            }

            if (!string.IsNullOrWhiteSpace(user.City))
            {
                existingUser.City = user.City;
            }

            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                existingUser.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound(new
                    {
                        status = 404,
                        message = "Usuário não encontrado.",
                        traceId = HttpContext.TraceIdentifier
                    });
                }
                throw;
            }

            return NoContent();
        }


        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new
                {
                    status = 404,
                    message = "Usuário não encontrado.",
                    traceId = HttpContext.TraceIdentifier
                });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(Guid id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}