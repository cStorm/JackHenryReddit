using System.Text;
using System.Text.Json;
using JackHenry.Reddit.Api.Contracts;

namespace JackHenry.Reddit.Api;

public abstract class ApiClient
{
    protected readonly string _baseUrl = "https://oauth.reddit.com";
    private readonly ApiCredentials _credentials;
    protected readonly string _userAgent;
    protected readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);
    private string? _accessToken;

    protected ApiClient(ApiCredentials credentials)
    {
        _credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
        _userAgent = GetType().FullName ?? throw new InvalidCastException("null");
    }

    protected string AuthorizationValue => "bearer " + (_accessToken ?? throw new InvalidOperationException());

    private async Task EnsureAccessToken()
    {
        if (_accessToken == null)
        {
            string baseUrl = "https://www.reddit.com";
            string url = $"/api/v1/access_token?grant_type=refresh_token&refresh_token=" + _credentials.RefreshToken;
            string authorization = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(_credentials.AppId + ":" + _credentials.AppSecret));
            string contentType = "application/x-www-form-urlencoded";

            AuthContract auth = await ExchangeRefreshToken(baseUrl, url, authorization, contentType);
            _accessToken ??= auth.access_token;
        }
    }

    public async Task<T> GetAsync<T>(string url, CancellationToken cancellationToken = default)
    {
        await EnsureAccessToken();
        await EnsureInitialized();
        return await GetObjectAsync<T>(url, cancellationToken);
    }

    protected abstract Task<AuthContract> ExchangeRefreshToken(string baseUrl, string url, string authorization, string contentType);
    protected abstract Task EnsureInitialized();
    protected abstract Task<T> GetObjectAsync<T>(string url, CancellationToken cancellationToken);
}
