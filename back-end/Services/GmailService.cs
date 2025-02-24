using back_end.Services.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Text;


public class GmailService : IEmailService
{
    private static readonly string[] Scopes = { Google.Apis.Gmail.v1.GmailService.Scope.GmailSend };
    private static readonly string ApplicationName = "Gmail API .NET Quickstart";
    private Stream stream;

    public async Task SendRecoveryEmail(string userEmail, string token)
    {
        UserCredential credential;

        // Carregar as credenciais
        var redirectUri = "http://localhost:7040"; // Coloque a URI que você registrou no Google aqui

        credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            GoogleClientSecrets.FromStream(stream).Secrets,
            Scopes,
            "user",
            CancellationToken.None,
            new FileDataStore("token.json", true),
            new LocalServerCodeReceiver(htp://localhost:7040) // Adicione a URI de redirecionamento aqui
        );

        // Cria o serviço do Gmail
        var service = new Google.Apis.Gmail.v1.GmailService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });

        // Monta o e-mail
        var emailMessage = $"From: me\r\nTo: {userEmail}\r\nSubject: Código de Recuperação de Senha\r\n\r\n" +
                           $"Seu código de recuperação é: {token}\n" +
                           "Este código é válido por 30 minutos.";
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