using back_end.Data;
using back_end.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
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

        // Método para fazer o upload de arquivos
        private async Task<string> UploadFile(IFormFile file, string folderName)
        {
            if (file == null)
                return null;

            string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            string folderPath = Path.Combine("uploads", folderName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = Path.Combine(folderPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            string baseUrl = $"{Request.Scheme}://{Request.Host}/";
            return $"{baseUrl}uploads/{folderName}/{fileName}";
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

            manual.ImageUrl = await UploadFile(request.ImageUrl, "images");
            manual.ManualPdfUrl = await UploadFile(request?.ManualPdfUrl, "manuals");
            manual.LaudoPdfUrl = await UploadFile(request.LaudoPdfUrl, "laudos");

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

            existingManual.Title = request.Title ?? existingManual.Title;
            existingManual.Description = request.Description ?? existingManual.Description;
            existingManual.VideoUrl = request.VideoUrl ?? existingManual.VideoUrl;

            if (request.ImageUrl != null)
            {
                existingManual.ImageUrl = await UploadFile(request.ImageUrl, "images");
            }
            if (request.ManualPdfUrl != null)
            {
                existingManual.ManualPdfUrl = await UploadFile(request.ManualPdfUrl, "manuals");
            }
            if (request.LaudoPdfUrl != null)
            {
                existingManual.LaudoPdfUrl = await UploadFile(request.LaudoPdfUrl, "laudos");
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