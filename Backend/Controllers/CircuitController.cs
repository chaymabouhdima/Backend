using Backend.Data;
using Backend.Models; // ← Model متاعك
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CircuitController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CircuitController(AppDbContext context)
        {
            _context = context;
        }

        // 🔹 Get All
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var circuits = await _context.Circuits.ToListAsync();
            return Ok(circuits);
        }

        // 🔹 Get By Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var circuit = await _context.Circuits.FindAsync(id);
            if (circuit == null)
                return NotFound();

            return Ok(circuit);
        }

        // 🔹 Create
        [HttpPost]
        public async Task<IActionResult> Create(Backend.Models.Circuit circuit)  // ← namespace كامل
        {
            _context.Circuits.Add(circuit);
            await _context.SaveChangesAsync();

            return Ok(circuit);
        }

        // 🔹 Update
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Backend.Models.Circuit updatedCircuit)
        {
            if (id != updatedCircuit.Id)
                return BadRequest();

            _context.Entry(updatedCircuit).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(updatedCircuit);
        }

        // 🔹 Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var circuit = await _context.Circuits.FindAsync(id);
            if (circuit == null)
                return NotFound();

            _context.Circuits.Remove(circuit);
            await _context.SaveChangesAsync();

            return Ok("Deleted successfully");
        }
    }
}