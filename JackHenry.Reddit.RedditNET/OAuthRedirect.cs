namespace JackHenry.Reddit.RedditNET;

public class OAuthRedirect
{
    public int Port { get; init; }
    public string GetUrl() => $"http://127.0.0.1:{Port}/Reddit.NET/oauthRedirect";
}
