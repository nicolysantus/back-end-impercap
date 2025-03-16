using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Google.Apis.Util;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class GmailServiceHelper
{
    private static readonly string[] Scopes = { Google.Apis.Gmail.v1.GmailService.Scope.GmailSend };
    private static readonly string ApplicationName = "Gmail API .NET Quickstart";

    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _tokenFilePath;

    public GmailServiceHelper(string clientId, string clientSecret, string tokenFilePath)
    {
        _clientId = clientId;
        _clientSecret = clientSecret;
        _tokenFilePath = tokenFilePath;
    }

    public async Task<Google.Apis.Gmail.v1.GmailService> GetGmailServiceAsync()
    {
        var credential = await GetUserCredentialAsync();
        return new Google.Apis.Gmail.v1.GmailService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });
    }

    private async Task<UserCredential> GetUserCredentialAsync()
    {
        UserCredential credential;

        // Carregar o token de um arquivo
        using (var stream = new FileStream(_tokenFilePath, FileMode.Open, FileAccess.Read))
        using (var reader = new StreamReader(stream))
        {
            var json = await reader.ReadToEndAsync();
            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(json);

            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = _clientId,
                    ClientSecret = _clientSecret
                }
            });

            // Cria a credencial do usuário
            credential = new UserCredential(flow, "user", tokenResponse);
        }

        // Verifica se o token está obsoleto e renova se necessário
        if (credential.Token.IsStale)
        {
            await credential.RefreshTokenAsync(CancellationToken.None);
        }

        return credential;
    }
}