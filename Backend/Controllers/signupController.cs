using Backend.Data;
using Backend.Model;
using Backend.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class SignupController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SignupController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("signup")]
        public IActionResult Signup([FromBody] RegisterDto dto)
        {
            // Validation des champs
            if (dto == null ||
                string.IsNullOrEmpty(dto.nom) ||
                string.IsNullOrEmpty(dto.email) ||
                string.IsNullOrEmpty(dto.mot_de_passe))
            {
                return BadRequest("Nom, email et mot de passe sont obligatoires.");
            }

            // Vérifier si email existe déjà
            bool userExists = _context.utilisateur.Any(u => u.email == dto.email);
            if (userExists)
                return BadRequest("Email déjà utilisé");

            // Création de l'utilisateur
            var user = new utilisateur
            {

                nom = dto.nom,
                email = dto.email,
                mot_de_passe = dto.mot_de_passe, // أو Hash إذا حابة
                role = dto.role
            };

            _context.utilisateur.Add(user);
            _context.SaveChanges();

            return Ok(new
            {
                message = "Utilisateur créé avec succès"
            });
        }
    }
}
