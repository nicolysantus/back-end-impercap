namespace back_end.Models
{
    public class PasswordResetToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? UserEmail { get; set; }
        public string? Code { get; set; }
        public DateTime Expiration { get; set; }
    }
}