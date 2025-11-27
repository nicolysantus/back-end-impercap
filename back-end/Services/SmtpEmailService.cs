using back_end.Services.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;
using DotNetEnv;

namespace back_end.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly string _host;
        private readonly int _port;
        private readonly string _user;
        private readonly string _pass;

        public SmtpEmailService()
        {
            // Tenta pegar do Environment (Docker injeta isso) ou carrega do .env localmente
            _host = Environment.GetEnvironmentVariable("SMTP_HOST") ?? Env.GetString("SMTP_HOST");
            _port = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? Env.GetString("SMTP_PORT") ?? "587");
            _user = Environment.GetEnvironmentVariable("SMTP_USER") ?? Env.GetString("SMTP_USER");
            _pass = Environment.GetEnvironmentVariable("SMTP_PASS") ?? Env.GetString("SMTP_PASS");
        }

        public async Task SendRecoveryEmail(string userEmail, string token)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Impercap Suporte", _user));
            message.To.Add(new MailboxAddress("", userEmail));
            message.Subject = "IMPERCAP SUPORT - RECUPERAR SENHA";

            var builder = new BodyBuilder();
            
            // Seu HTML original preservado
            builder.HtmlBody = $@"
            <html>
            <body style=""font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px; text-align: center;"">
                <div style=""max-width: 500px; margin: auto; background: #cfefff; padding: 20px; border-radius: 10px; box-shadow: 2px 2px 10px rgba(0,0,0,0.1);"">
                    <h2 style=""color: #A23067; text-align: center; font-weight: bold;"">Impercap Suporte</h2>
                    <p style=""color: #333; font-size: 16px; text-align: left;"">Olá,<br>Recebemos à sua solicitação de redefinir senha.</p>
                    <h2 style=""color: #A23067; text-align: center; font-size: 16px; font-weight: bold;"">Seu código de recuperação é:</h2>
                    <div style=""display: inline-block; background: #A23067; color: #fff; font-size: 22px; font-weight: bold; padding: 15px 25px; border-radius: 10px; margin: 10px 0;"">
                        {token}
                    </div>
                    <p style=""color: #333; font-size: 14px; text-align: left;"">Aviso:<br>O código expira em 30 minutos.</p>
                    <p style=""color: #333; font-size: 14px; text-align: left;"">Atenciosamente,<br><strong>Equipe Impercap Suporte</strong></p>
                </div>
            </body>
            </html>";

            message.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                // Ignora erro de SSL em desenvolvimento
                client.CheckCertificateRevocation = false;
                
                await client.ConnectAsync(_host, _port, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_user, _pass);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}