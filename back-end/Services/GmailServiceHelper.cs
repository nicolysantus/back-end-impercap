using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;

public class GmailServiceHelper
{
    private static readonly string[] Scopes = { Google.Apis.Gmail.v1.GmailService.Scope.GmailSend };
    private static readonly string ApplicationName = "Gmail API .NET Quickstart";

    private readonly string _clientId;
    private readonly string _clientSecret;

    public GmailServiceHelper(string clientId, string clientSecret)
    {
        _clientId = clientId;
        _clientSecret = clientSecret;
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
        var secrets = new ClientSecrets
        {
            ClientId = _clientId,
            ClientSecret = _clientSecret
        };

        var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            secrets,
            Scopes,
            "user",
            CancellationToken.None,
            new FileDataStore("token.json", true)
        );

        return credential;
    }
}