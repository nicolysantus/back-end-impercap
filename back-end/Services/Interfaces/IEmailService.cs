namespace back_end.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendRecoveryEmail(string userEmail, string token);
    }
}