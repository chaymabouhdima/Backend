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
            try
            {
                if (dto == null || string.IsNullOrEmpty(dto.email) || string.IsNullOrEmpty(dto.mot_de_passe))
                    return BadRequest(new { message = "Email et mot de passe sont obligatoires." });

                var dbUser = _context.utilisateur.FirstOrDefault(x => x.email == dto.email);

                if (dbUser == null || dto.mot_de_passe != dbUser.mot_de_passe)
                    return Unauthorized(new { message = "Email ou mot de passe invalide" });

                var token = GenerateToken(dbUser);

                return Ok(new { token, role = dbUser.role });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur interne: " + ex.Message });
            }
        }
       


        [HttpGet("user")]
        public IActionResult GetUtilisateurs(int page = 1, int pageSize = 5)
        {
            var query = _context.utilisateur
                .Where(u => u.role == "utilisateur");

            var totalCount = query.Count();

            var users = query
                .OrderBy(u => u.id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new
                {
                    u.id,
                    u.nom,
                    u.email
                  
                })
                .ToList();

            return Ok(new
            {
                totalCount,
                data = users
            });
        }
        [HttpDelete("utilisateur/{id}")]
        public IActionResult DeleteUtilisateur(int id)
        {
            var user = _context.utilisateur.FirstOrDefault(u => u.id == id);

            if (user == null)
                return NotFound(new { message = "Utilisateur non trouvé" });

            _context.utilisateur.Remove(user);
            _context.SaveChanges();

            return Ok(new { message = "Utilisateur supprimé avec succès" });
        }

        [HttpPut("utilisateur/{id}")]
        public IActionResult UpdateUtilisateur(int id, [FromBody] UpdateUtilisateurDto user)
        {
            if (user == null || string.IsNullOrEmpty(user.email) || string.IsNullOrEmpty(user.nom))
                return BadRequest(new { message = "Email et nom sont obligatoires" });

            var query = _context.utilisateur.FirstOrDefault(u => u.id == id);

            if (query == null)
                return NotFound(new { message = "Utilisateur non trouvé" });

            query.email = user.email;
            query.nom = user.nom;

            _context.SaveChanges();

            return Ok(new { message = "Utilisateur mis à jour" });
        }

        [HttpPost("utilisateur")]
        public IActionResult AddUtilisateur([FromBody] utilisateur user)
        {
            if (user == null || string.IsNullOrEmpty(user.nom) || string.IsNullOrEmpty(user.email))
                return BadRequest(new { message = "Nom et email sont obligatoires." });

            _context.utilisateur.Add(user);
            _context.SaveChanges();

            return Ok(user); // نرجعو المستخدم الجديد باش يظهر مباشرة في frontend
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
