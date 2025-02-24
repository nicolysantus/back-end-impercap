using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;

public class GmailServiceHelper
{
    private static readonly string[] Scopes = { Google.Apis.Gmail.v1.GmailService.Scope.GmailSend };
    private static readonly string ApplicationName = "Gmail API .NET Quickstart";
    private readonly string _credentialPath;
    private readonly string _tokenPath;

    public GmailServiceHelper(string credentialPath, string tokenPath)
    {
        _credentialPath = credentialPath;
        _tokenPath = tokenPath;
    }

    public async Task<UserCredential> GetUserCredentialAsync()
    {
        UserCredential credential;

        using (var stream = new FileStream(_credentialPath, FileMode.Open, FileAccess.Read))
        {
            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(_tokenPath, true));
        }

        return credential;
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
}