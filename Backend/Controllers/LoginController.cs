using Backend.Data;
using Backend.Model;
using Backend.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public LoginController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.email) || string.IsNullOrEmpty(dto.mot_de_passe))
                return BadRequest("Email et mot de passe sont obligatoires.");

            var dbUser = _context.utilisateur
                .FirstOrDefault(x => x.email == dto.email);

            if (dbUser == null)
                return Unauthorized("Email ou mot de passe invalide");

            if (dto.mot_de_passe != dbUser.mot_de_passe)
                return Unauthorized("Email ou mot de passe invalide");

            var token = GenerateToken(dbUser);
            return Ok(new { token, role= dbUser.role });
        }
        [HttpGet("user/{id}")]
        public IActionResult GetUserById(long id)
        {
            var user = _context.utilisateur
                .Where(u => u.id == id)
                .Select(u => new { u.id, u.nom, u.email })
                .FirstOrDefault();

            if (user == null)
                return NotFound("Utilisateur non trouvé");

            return Ok(user);
        }
        private string GenerateToken(utilisateur user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                new Claim(ClaimTypes.Email, user.email),
               
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"])
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
