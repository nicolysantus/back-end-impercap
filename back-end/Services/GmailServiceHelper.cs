using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
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
    private readonly string _accessToken;
    private readonly string _refreshToken;

    public GmailServiceHelper(string clientId, string clientSecret, string accessToken, string refreshToken)
    {
        _clientId = clientId;
        _clientSecret = clientSecret;
        _accessToken = accessToken;
        _refreshToken = refreshToken;
    }

    // Método que retorna uma instância de GmailService
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
        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = _clientId,
                ClientSecret = _clientSecret
            }
        });

        var tokenResponse = new TokenResponse
        {
            AccessToken = _accessToken,
            RefreshToken = _refreshToken,
            ExpiresInSeconds = 3599 // Utilize o valor de expiração que você tem
        };

        var credential = new UserCredential(flow, "user", tokenResponse);

        if (credential.Token.IsStale)
        {
            await credential.RefreshTokenAsync(CancellationToken.None);
        }

        return credential;
    }
}