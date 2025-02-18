using back_end.Data;
using back_end.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
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

        // GET: api/manual
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ManualModel>>> GetManuais()
        {
            return await _context.Manuals.ToListAsync();
        }

        // GET: api/manual/{id}
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

        // POST: api/manual
        [HttpPost]
        public async Task<ActionResult<ManualModel>> CreateManual([FromForm] CreateManualRequest request)

        {
            var manual = new ManualModel
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                VideoUrl = request.VideoUrl,
            };

            string imagePath = Path.Combine("uploads/images");
            string manualPath = Path.Combine("uploads/manuals");
            string laudoPath = Path.Combine("uploads/laudos");


            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }

            if (!Directory.Exists(manualPath))
            {
                Directory.CreateDirectory(manualPath);
            }

            if (!Directory.Exists(laudoPath))
            {
                Directory.CreateDirectory(laudoPath);
            }

            if (request.ImageUrl != null)
            {
                var fullImagePath = Path.Combine(imagePath, request.ImageUrl.FileName);

                using (var stream = new FileStream(fullImagePath, FileMode.Create))
                {
                    await request.ImageUrl.CopyToAsync(stream);
                }
                manual.ImageUrl = fullImagePath;

            }

            if (request.ManualPdfUrl != null)
            {
                var fullManualPath = Path.Combine(manualPath, request.ManualPdfUrl.FileName);

                using (var stream = new FileStream(fullManualPath, FileMode.Create))

                {
                    await request.ManualPdfUrl.CopyToAsync(stream);
                }

                manual.ManualPdfUrl = fullManualPath; 

            }

            if (request.LaudoPdfUrl != null)

            {

                var fullLaudoPath = Path.Combine(laudoPath, request.LaudoPdfUrl.FileName);

                using (var stream = new FileStream(fullLaudoPath, FileMode.Create))

                {

                    await request.LaudoPdfUrl.CopyToAsync(stream);

                }

                manual.LaudoPdfUrl = fullLaudoPath; 

            }

            _context.Manuals.Add(manual);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetManual), new { id = manual.Id }, manual);

        }


        // PUT: api/manual/{id}

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateManual(Guid id, [FromForm] CreateManualRequest request)
        {
            var existingManual = await _context.Manuals.FindAsync(id);
            if (existingManual == null)
            {
                return NotFound();
            }

            if (request.Title != null)
            {
                existingManual.Title = request.Title;
            }

            if (request.Description != null)
            {
                existingManual.Description = request.Description;
            }

            if (request.VideoUrl != null)
            {
                existingManual.VideoUrl = request.VideoUrl;
            }

            string imagePath = Path.Combine("uploads/images");
            string manualPath = Path.Combine("uploads/manuals");
            string laudoPath = Path.Combine("uploads/laudos");

            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }

            if (!Directory.Exists(manualPath))
            {
                Directory.CreateDirectory(manualPath);
            }

            if (!Directory.Exists(laudoPath))
            {
                Directory.CreateDirectory(laudoPath);
            }

            if (request.ImageUrl != null)
            {
                var fullImagePath = Path.Combine(imagePath, request.ImageUrl.FileName);
                using (var stream = new FileStream(fullImagePath, FileMode.Create))
                {
                    await request.ImageUrl.CopyToAsync(stream);
                }
                existingManual.ImageUrl = fullImagePath;
            }

            if (request.ManualPdfUrl != null)
            {
                var fullManualPath = Path.Combine(manualPath, request.ManualPdfUrl.FileName);
                using (var stream = new FileStream(fullManualPath, FileMode.Create))
                {
                    await request.ManualPdfUrl.CopyToAsync(stream);
                }
                existingManual.ManualPdfUrl = fullManualPath;
            }

            if (request.LaudoPdfUrl != null)
            {
                var fullLaudoPath = Path.Combine(laudoPath, request.LaudoPdfUrl.FileName);
                using (var stream = new FileStream(fullLaudoPath, FileMode.Create))
                {
                    await request.LaudoPdfUrl.CopyToAsync(stream);
                }
                existingManual.LaudoPdfUrl = fullLaudoPath;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }


        // DELETE: api/manual/{id}

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
