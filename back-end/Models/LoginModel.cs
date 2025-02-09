namespace back_end.Models
{
    public class LoginModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
