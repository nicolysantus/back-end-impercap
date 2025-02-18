namespace back_end.Models
{
    public class ManualModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; } 
        public string? ManualPdfUrl { get; set; } 
        public string? VideoUrl { get; set; } 
        public string? LaudoPdfUrl { get; set; }
    }
    public class CreateManualRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public IFormFile? ImageUrl { get; set; }
        public IFormFile? ManualPdfUrl { get; set; }
        public string? VideoUrl { get; set; }
        public IFormFile? LaudoPdfUrl { get; set; }
    }

}
