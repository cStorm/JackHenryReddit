using CommandLine;
using JackHenry.Reddit.RedditNET;

namespace JackHenry.Reddit.ConsoleApp;

[Verb("auth", HelpText = "Get refresh token using OAuth.")]
public class Authorize : ICommand
{
    [Value(0, MetaName = "app-id", Required = true)]
    public string? AppId { get; set; }

    [Value(1, MetaName = "app-secret")]
    public string? AppSecret { get; set; }

    [Option(Default = 8080)]
    public int Port { get; set; }

    public void Execute()
    {
        if (AppId == null) throw new ArgumentNullException(nameof(AppId));

        Authorizer authorizer = new(Port, AppId, AppSecret);
        Console.WriteLine($"Make sure you are logged into Reddit and your app's redirect_uri is {authorizer.RedirectUri}");
        Console.WriteLine("Press any key when ready...");
        Console.ReadKey();

        authorizer.Authorize();
        Console.WriteLine();
        Console.WriteLine($"refresh_token: {authorizer.RefreshToken}");
    }
}