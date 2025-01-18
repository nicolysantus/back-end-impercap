using back_end.Data;
using back_end.Models;  
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace back_end.API.Controllers
{
    [Route("api/[controller]")]
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
        public async Task<ActionResult<ManualModel>> GetManual(int id)
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
            _context.Manuals.Add(manual);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetManual), new { id = manual.Id }, manual);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateManual(int id, ManualModel manual)
        {
            if (id != manual.Id)
            {
                return BadRequest();
            }

            _context.Entry(manual).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteManual(int id)
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
    }
}
