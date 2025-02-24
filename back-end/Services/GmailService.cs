using back_end.Services.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Text;


public class GmailService : IEmailService
{
    private readonly GmailServiceHelper _gmailServiceHelper;

    public GmailService(string credentialPath, string tokenPath)
    {
        _gmailServiceHelper = new GmailServiceHelper(credentialPath, tokenPath);
    }

    public async Task SendRecoveryEmail(string userEmail, string token)
    {
        var service = await _gmailServiceHelper.GetGmailServiceAsync();

        // Monta o e-mail em HTML
        var emailBody = $@"
    <html>
    <body>
        <h2>Impercap Suporte</h2>
        <p>Seu código de recuperação é: <strong>{token}</strong></p>
        <p>Este código é válido por 30 minutos.</p>
        <hr>
        <p>Atenciosamente,<br>Equipe Impercap Suporte</p>
        <p><img src='https://www.impercap.com.br/template/imagens/logo.png' alt='Logo Impercap' style='width:100px;height:auto;'></p>
        <p><em>Mensagem de segurança: Nunca compartilhe seu código de recuperação com ninguém.</em></p>
    </body>
    </html>";

        var emailMessage = $"From: \"Impercap Suporte\" <aplicativoimpercap@gmail.com>\r\n" +
                           $"To: {userEmail}\r\n" +
                           "Subject: IMPERCAP SUPORT - RECUPERAR SENHA\r\n" +
                           "Content-Type: text/html; charset=UTF-8\r\n\r\n" +
                           emailBody;

        var message = new Message
        {
            Raw = Base64UrlEncode(emailMessage)
        };

        // Envia o e-mail
        await service.Users.Messages.Send(message, "me").ExecuteAsync();
        Console.WriteLine("E-mail enviado com sucesso.");
    }

    private static string Base64UrlEncode(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        return Convert.ToBase64String(bytes)
        .Replace("+", "-")
        .Replace("/", "_")
        .Replace("=", "");
    }
}