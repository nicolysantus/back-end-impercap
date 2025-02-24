namespace back_end.Models.Request
{
    public class ResetPasswordRequest
    {
        public string? Code { get; set; }
        public string? NewPassword { get; set; }
    }
}