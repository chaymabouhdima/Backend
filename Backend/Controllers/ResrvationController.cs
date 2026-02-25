using Backend.Data;
using Backend.DTO;
using Backend.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


[ApiController]
[Route("api/rese")]
public class ReservationsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ReservationsController(AppDbContext context)
    {
        _context = context;
    }

    // GET ALL
    [HttpGet("all-reservations")]
    public async Task<IActionResult> GetAllReservations()
    {
        var reservations = await _context.Reservations.ToListAsync();
        return Ok(reservations);
    }

    // GET BY USER ID (without token)
    [HttpGet("my-reservations/{userId}")]
    public async Task<IActionResult> GetMyReservations(int userId)
    {
        var reservations = await _context.Reservations
            .Where(r => r.userId == userId)
            .ToListAsync();

        return Ok(reservations);
    }

    // CREATE
    [HttpPost]
    public async Task<IActionResult> CreateReservation(CreateReservationDto dto)
    {
        var reservation = new reservation
        {
            userId = dto.UserId, // pass from frontend
            CircuitId = dto.CircuitId,
            DateReservation = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        return Ok(reservation);
    }

    // DELETE
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReservation(int id)
    {
        var reservation = await _context.Reservations
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reservation == null)
            return NotFound(new { message = "Reservation non trouvée." });

        _context.Reservations.Remove(reservation);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Reservation supprimée avec succès." });
    }
}