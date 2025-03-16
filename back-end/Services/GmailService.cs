using back_end.Services.Interfaces;
using Google.Apis.Gmail.v1.Data;
using System.Text;

public class GmailService : IEmailService
{
    private readonly GmailServiceHelper _gmailServiceHelper;

    public GmailService(string clientId, string clientSecret, string accessToken, string refreshToken)
    {
        _gmailServiceHelper = new GmailServiceHelper(clientId, clientSecret, accessToken, refreshToken);
    }

    public async Task SendRecoveryEmail(string userEmail, string token)
    {
        var service = await _gmailServiceHelper.GetGmailServiceAsync();

        // Monta o e-mail em HTML
        var emailBody = $@"
<html>
<body style=""font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px; text-align: center;"">

    <div style=""max-width: 500px; margin: auto; background: #cfefff; padding: 20px; border-radius: 10px; box-shadow: 2px 2px 10px rgba(0,0,0,0.1);"">

        <!-- Imagem do topo -->
        <div style=""max-width: 500px; margin: auto; border-radius: 10px; overflow: hidden;"">
            <img src=""https://i.ibb.co/whqCn8ZT/logo-topo.png""
                 alt=""Impercap Banner"" style=""width: 100%; max-width: 500px; display: block; margin: auto; border-radius: 10px;"">
        </div>

        <h2 style=""color: #A23067; text-align: center; font-weight: bold;"">Impercap Suporte</h2>

        <p style=""color: #333; font-size: 16px; text-align: left;"">
            Olá,<br>Recebemos à sua solicitação de redefinir senha.
        </p>

        <h2 style=""color: #A23067; text-align: center; font-size: 16px; font-weight: bold;"">Seu código de recuperação é:</h2>

        <!-- Código dentro de um círculo -->
        <div style=""display: inline-block; background: #A23067; color: #fff; font-size: 22px; font-weight: bold; padding: 15px 25px; border-radius: 10px; margin: 10px 0;"">
            {token}
        </div>

        <p style=""color: #333; font-size: 14px; text-align: left;"">
            Aviso:<br>
             - O código é pessoal e de uso único, não deve ser divulgado.<br>
            <span style=""color: #A23067; font-weight: bold;""> - Ele expira em 30 minutos.</span> Caso não utilize dentro do prazo, será necessário solicitar um novo código.
        </p>

        <p style=""color: #333; font-size: 14px; text-align: left;"">
            Se você não solicitou essa alteração, desconsidere este e-mail.
        </p>

        <hr style=""border: 0; height: 1px; background: #bbb; margin: 20px 0;"">

        <p style=""color: #333; font-size: 14px; text-align: left;"">
            Se precisar de ajuda, estamos à disposição.<br>
            Atenciosamente,<br><strong>Equipe Impercap Suporte</strong>
        </p>

        <!-- Logo da empresa no final -->
        <div style=""margin-top: 20px; border-radius: 10px; overflow: hidden;"">
            <img src=""https://media.licdn.com/dms/image/v2/D4D16AQH7Lo9ei5oK_Q/profile-displaybackgroundimage-shrink_350_1400/profile-displaybackgroundimage-shrink_350_1400/0/1728296682232?e=1746057600&v=beta&t=_KUWHTqMVD-brzBpU_OONLkzz_r7kdHkJ0Sh3pC7SII""
                 alt=""Logo Impercap"" style=""width: 100%; max-width: 400px; display: block; margin: auto; border-radius: 10px;"">
        </div>

    </div>

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