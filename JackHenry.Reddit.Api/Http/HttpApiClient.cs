using JackHenry.Reddit.Api.Contracts;
using System.Text.Json;

namespace JackHenry.Reddit.Api.Http;

public class HttpApiClient : ApiClient, IDisposable
{
    private readonly HttpClient _client;

    public HttpApiClient(ApiCredentials credentials) : base(credentials)
    {
        _client = CreateHttpClient();
    }

    private HttpClient CreateHttpClient(string? baseUrl = null)
    {
        return new()
        {
            BaseAddress = new Uri(baseUrl ?? _baseUrl),
            DefaultRequestHeaders = { { "User-Agent", _userAgent } }
        };
    }


    protected override async Task<AuthContract> ExchangeRefreshToken(string baseUrl, string url, string authorization, string contentType)
    {
        using HttpClient client = CreateHttpClient(baseUrl);
        client.DefaultHeader("Authorization", authorization);

        //{ "Content-Type", contentType }
        using HttpResponseMessage response = await client.PostAsync(url, null);
        response.EnsureSuccessStatusCode();

        return await DeserializeAsync<AuthContract>(response);
    }

    protected override Task EnsureInitialized()
    {
        _client.DefaultHeader("Authorization", AuthorizationValue);
        return Task.CompletedTask;
    }

    protected override async Task<T> GetObjectAsync<T>(string url)
    {
        using HttpResponseMessage response = await _client.GetAsync(url);
        VerifyResponseMessage(response);

        return await DeserializeAsync<T>(response);
    }

    private async Task<T> DeserializeAsync<T>(HttpResponseMessage response)
    {
        using Stream stream = await response.Content.ReadAsStreamAsync() ?? throw new InvalidCastException("no stream");
        return JsonSerializer.Deserialize<T>(stream, _jsonOptions) ?? throw new InvalidCastException("null");
    }

    private void VerifyResponseMessage(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        int remaining = (int)decimal.Parse(response.GetHeader("x-ratelimit-remaining")!);
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}
