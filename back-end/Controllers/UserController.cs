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
        public IActionResult Register([FromBody] UserModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                // Retorna as mensagens de erro de validação
                return BadRequest(ModelState.Values.SelectMany(v => v.Errors)
                                                    .Select(e => e.ErrorMessage));
            }

            // Verifica se o usuário já existe pelo nome de usuário
            var existingUserByUsername = _context.Users.SingleOrDefault(u => u.Username == registerModel.Username);
            if (existingUserByUsername != null)
            {
                return Conflict("O nome de usuário já está em uso."); // Retorna conflito se o nome de usuário já existir
            }

            // Verifica se o CPF já existe
            var existingUserByCpf = _context.Users.SingleOrDefault(u => u.CPF == registerModel.CPF);
            if (existingUserByCpf != null)
            {
                return Conflict("O CPF já está em uso."); // Retorna conflito se o CPF já existir
            }

            // Cria um novo usuário
            var newUser = new UserModel
            {
                Username = registerModel.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(registerModel.CPF), // Define a senha como o CPF
                CPF = registerModel.CPF,
                FirstName = registerModel.FirstName,
                LastName = registerModel.LastName,
                DateOfBirth = registerModel.DateOfBirth,
                Email = registerModel.Email,
                Address = registerModel.Address,
                Number = registerModel.Number,
                Neighborhood = registerModel.Neighborhood,
                City = registerModel.City,
                UserType = registerModel.UserType
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return Ok(new
            {
                status = 200,
                message = "Usuário registrado com sucesso.",
                traceId = HttpContext.TraceIdentifier
            });
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(Guid id, UserModel user)
        {
            if (id != user.Id)
            {
                return BadRequest(new
                {
                    status = 400,
                    message = "O ID do usuário fornecido não corresponde ao ID do usuário a ser atualizado.",
                    traceId = HttpContext.TraceIdentifier
                });
            }

            // Se a senha foi fornecida, criptografa a nova senha
            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            }
            else
            {
                // Mantém a senha existente no banco
                var existingUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
                if (existingUser != null)
                {
                    user.Password = existingUser.Password;
                }
            }

            _context.Entry(user).State = EntityState.Modified;

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