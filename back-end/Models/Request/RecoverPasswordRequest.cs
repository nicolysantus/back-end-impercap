namespace back_end.Models.Request
{
    public class RecoverPasswordRequest
    {
        public string? Username { get; set; }
        public string? DateOfBirth { get; set; }
        public string? CPF { get; set; }
    }
}