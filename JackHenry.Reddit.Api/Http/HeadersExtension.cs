namespace JackHenry.Reddit.Api.Http;

public static class HeadersExtension
{
    public static void DefaultHeader(this HttpClient client, string name, string value)
    {
        if (!client.DefaultRequestHeaders.Contains(name))
            client.DefaultRequestHeaders.Add(name, value);
    }

    public static string? GetHeader(this HttpResponseMessage response, string name)
    {
        return response.Headers.GetValues(name).FirstOrDefault();
    }
}
