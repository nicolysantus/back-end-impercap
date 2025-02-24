using back_end.Data;
using back_end.Models;
using back_end.Models.Request;
using back_end.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace back_end.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthController(AppDbContext context, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            if (loginModel == null || string.IsNullOrEmpty(loginModel.Username) || string.IsNullOrEmpty(loginModel.Password))
            {
                return BadRequest("Usuário e senha são obrigatórios.");
            }

            var user = _context.Users.SingleOrDefault(u => u.Username == loginModel.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginModel.Password, user.Password))
            {
                return Unauthorized("Credenciais inválidas.");
            }

            var token = GenerateJwtToken(user);

            if (loginModel.Password == user.CPF)
            {
                return Ok(new { CPF = user.CPF });
            }
            else
            {
                return Ok(new { Token = token });
            }
        }

        private string GenerateJwtToken(UserModel user)
        {
            var key = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(key))
            {
                throw new InvalidOperationException("JWT Key is missing or empty in configuration.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username ?? "unknown"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("recover")]
        public async Task<IActionResult> RecoverPassword([FromBody] RecoverPasswordRequest request) // Método agora é assíncrono
        {
            if (request == null)
            {
                return BadRequest("A solicitação não pode ser nula.");
            }

            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.DateOfBirth) || string.IsNullOrEmpty(request.CPF))
            {
                return BadRequest("Usuário, data de nascimento e CPF são obrigatórios.");
            }

            var user = _context.Users.SingleOrDefault(u => u.Username == request.Username && u.DateOfBirth == request.DateOfBirth && u.CPF == request.CPF);

            if (user == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            if (string.IsNullOrEmpty(user.Email))
            {
                return BadRequest("O e-mail do usuário não está disponível.");
            }

            var code = new Random().Next(100000, 999999).ToString();

            await _emailService.SendRecoveryEmail(user.Email, code);

            var expiration = DateTime.UtcNow.AddMinutes(30);
            _context.PasswordResetTokens.Add(new PasswordResetToken { UserEmail = user.Email, Code = code, Expiration = expiration });
            _context.SaveChanges();

            return Ok("Código de recuperação enviado ao seu e-mail.");
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Code) || string.IsNullOrEmpty(request.NewPassword))
            {
                return BadRequest("Código e nova senha são obrigatórios.");
            }

            var tokenRecord = _context.PasswordResetTokens.SingleOrDefault(t => t.Code == request.Code && t.Expiration > DateTime.UtcNow);

            if (tokenRecord == null)
            {
                return Unauthorized("Código inválido ou expirado.");
            }

            var user = _context.Users.SingleOrDefault(u => u.Email == tokenRecord.UserEmail);
            if (user == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            _context.SaveChanges();

            _context.PasswordResetTokens.Remove(tokenRecord);
            _context.SaveChanges();

            return Ok("Senha redefinida com sucesso.");
        }
    }
}