using back_end.Data;
using back_end.Models;
using back_end.Models.Request;
using back_end.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DotNetEnv;

namespace back_end.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        public AuthController(AppDbContext context, IEmailService emailService)
        {
            _context = context;
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

            try 
            {
                var token = GenerateJwtToken(user);

                var response = new
                {
                    UserId = user.Id,
                    Token = token,
                    CPF = (loginModel.Password == user.CPF) ? user.CPF : null
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Isso vai ajudar a ver o erro real no console se acontecer
                Console.WriteLine($"Erro ao gerar token: {ex.Message}");
                return StatusCode(500, "Erro interno ao gerar autenticação.");
            }
        }

        private string GenerateJwtToken(UserModel user)
        {
            // CORREÇÃO: Lê as variáveis do .env (Environment)
            var key = Environment.GetEnvironmentVariable("JWT_KEY");
            var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
            var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

            if (string.IsNullOrEmpty(key))
            {
                throw new InvalidOperationException("CRÍTICO: JWT_KEY não encontrada nas variáveis de ambiente.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username ?? "unknown"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("recover")]
        public async Task<IActionResult> RecoverPassword([FromBody] RecoverPasswordRequest request)
        {
            if (request == null) return BadRequest("A solicitação não pode ser nula.");
            
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.DateOfBirth) || string.IsNullOrEmpty(request.CPF))
            {
                return BadRequest("Usuário, data de nascimento e CPF são obrigatórios.");
            }

            var user = _context.Users.SingleOrDefault(u => u.Username == request.Username && u.DateOfBirth == request.DateOfBirth && u.CPF == request.CPF);

            if (user == null) return NotFound("Usuário não encontrado.");
            
            if (string.IsNullOrEmpty(user.Email)) return BadRequest("O e-mail do usuário não está disponível.");

            var code = new Random().Next(100000, 999999).ToString();

            // O EmailService já foi refatorado para usar .env internamente, então só chamamos:
            await _emailService.SendRecoveryEmail(user.Email, code);

            var expiration = DateTime.UtcNow.AddMinutes(30);
            _context.PasswordResetTokens.Add(new PasswordResetToken { UserEmail = user.Email, Code = code, Expiration = expiration });
            await _context.SaveChangesAsync();

            string maskedEmail = MaskEmail(user.Email);
            return Ok($"Código de recuperação enviado ao seu e-mail: {maskedEmail}.");
        }

        private string MaskEmail(string email)
        {
            var parts = email.Split('@');
            if (parts.Length < 2) return email; // Proteção contra email inválido
            
            var username = parts[0];
            var domain = parts[1];

            if (username.Length <= 4) return $"{username[0]}***@{domain}";

            string maskedUsername = $"{username.Substring(0, 2)}***{username.Substring(username.Length - 2)}";
            return $"{maskedUsername}@{domain}";
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Code) || string.IsNullOrEmpty(request.NewPassword))
            {
                return BadRequest("Código e nova senha são obrigatórios.");
            }

            var tokenRecord = _context.PasswordResetTokens.SingleOrDefault(t => t.Code == request.Code && t.Expiration > DateTime.UtcNow);

            if (tokenRecord == null) return Unauthorized("Código inválido ou expirado.");

            var user = _context.Users.SingleOrDefault(u => u.Email == tokenRecord.UserEmail);
            if (user == null) return NotFound("Usuário não encontrado.");

            user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            
            // Remove o token usado
            _context.PasswordResetTokens.Remove(tokenRecord);
            _context.SaveChanges(); 

            return Ok("Senha redefinida com sucesso.");
        }
    }
}