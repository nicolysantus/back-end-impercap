using back_end.Data;
using back_end.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace back_end.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public string Authenticate(string username, string password)
        {
            var foundUser = _context.Users.SingleOrDefault(u => u.Username == username && u.Password == password);
            if (foundUser == null)
            {
                return null;
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, foundUser.FirstName),
                new Claim(ClaimTypes.NameIdentifier, foundUser.Id.ToString()),
                new Claim(ClaimTypes.Role, foundUser.UserType ? "Admin" : "User")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("XkFj8NQv9wJ5M8uV2h3XpZzF5h7Vz3kD4y8QkBf7N8D4fW9qG7Tz3uD2b9Xz8qz"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "ImpercapAPI",
                audience: "ImpercapApp",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
