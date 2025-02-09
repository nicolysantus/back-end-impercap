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
    public class ManualController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ManualController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ManualModel>>> GetManuais()
        {
            return await _context.Manuals.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ManualModel>> GetManual(Guid id)
        {
            var manual = await _context.Manuals.FindAsync(id);

            if (manual == null)
            {
                return NotFound();
            }

            return manual;
        }

        [HttpPost]
        public async Task<ActionResult<ManualModel>> CreateManual(ManualModel manual)
        {
            manual.Id = Guid.NewGuid(); // Garante um ID único ao criar um novo registro
            _context.Manuals.Add(manual);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetManual), new { id = manual.Id }, manual);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateManual(Guid id, ManualModel manual)
        {
            if (id != manual.Id) // Comparação correta de Guids
            {
                return BadRequest("O ID informado não corresponde ao ID do manual.");
            }

            _context.Entry(manual).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ManualExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteManual(Guid id)
        {
            var manual = await _context.Manuals.FindAsync(id);
            if (manual == null)
            {
                return NotFound();
            }

            _context.Manuals.Remove(manual);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool ManualExists(Guid id)
        {
            return _context.Manuals.Any(e => e.Id == id);
        }
    }
}