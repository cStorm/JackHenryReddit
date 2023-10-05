using Reddit.AuthTokenRetriever;
using Reddit.AuthTokenRetriever.EventArgs;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace JackHenry.Reddit.RedditNET;

public class Authorizer
{
    private readonly AuthTokenRetrieverLib _retriever;
    private AuthSuccessEventArgs? _success = null;

    public Authorizer(int port, string appId, string? appSecret = null)
    {
        RedirectUri = $"http://127.0.0.1:{port}/Reddit.NET/oauthRedirect";
        _retriever = new AuthTokenRetrieverLib(appId, port, appSecret: appSecret);
        _retriever.AuthSuccess += OnSuccess;
    }

    public string RedirectUri { get; init; }
    public string RefreshToken => _retriever.RefreshToken;

    public void Authorize()
    {
        _retriever.AwaitCallback(false);
        try
        {
            _success = null;
            OpenBrowser(_retriever.AuthURL());
            while (_success == null)
                Thread.Sleep(1000);
        }
        finally
        {
            _retriever.StopListening();
        }
    }

    private void OnSuccess(object? sender, AuthSuccessEventArgs e) => _success = e;

    private static void OpenBrowser(string url)
    {
        ProcessStartInfo processStartInfo =
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? new(url) { UseShellExecute = true }
            : RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? new("open", url)
            : RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? new("xdg-open", url)
            : throw new NotSupportedException();
        Process.Start(processStartInfo);
    }
}
