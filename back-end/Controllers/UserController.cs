using back_end.Data;
using back_end.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace back_end.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserModel>>> GetUsers()
        {
            // Busca os usuários do banco de dados
            var users = await _context.Users.ToListAsync();

            // Recriptografa as senhas (não necessário em produção, apenas ilustrativo)
            foreach (var user in users)
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            }

            return users;
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<UserModel>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            // Retorna o usuário com a senha já criptografada
            return user;
        }


        [HttpPost("register")]
        public async Task<ActionResult<bool>> CreateUser(UserModel user)
        {
            // Verificar se o username ou CPF já existem no banco de dados
            if (_context.Users.Any(u => u.Username == user.Username || u.CPF == user.CPF))
            {
                return Conflict("Já existe um usuário com este nome de usuário ou CPF.");
            }

            // Criptografar a senha
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            // Salvar o usuário no banco de dados
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return true; // Retorna "true" indicando que a criação foi bem-sucedida
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserModel user)
        {
            if (id != user.Id)
            {
                return BadRequest("O ID do usuário não corresponde.");
            }

            // Se a senha não estiver vazia, criptografe a nova senha
            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Users.Any(e => e.Id == id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
