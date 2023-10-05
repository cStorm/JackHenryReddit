using CommandLine;
using JackHenry.Reddit.RedditNET;

namespace JackHenry.Reddit.ConsoleApp;

[Verb("watch", HelpText = "Show statistics for a subreddit.")]
public class Watch : ICommand
{
    [Value(0, MetaName = "subreddit", Required = true)]
    public string? Subreddit { get; set; }

    [Value(1, MetaName = "app-id", Required = true)]
    public string? AppId { get; set; }

    [Value(2, MetaName = "app-secret")]
    public string? AppSecret { get; set; }

    [Option('r', "refresh", MetaValue="refresh-token", Required = true)]
    public string? RefreshToken { get; set; }

    public void Execute()
    {
        if (Subreddit == null) throw new ArgumentNullException(nameof(Subreddit));
        if (AppId == null) throw new ArgumentNullException(nameof(AppId));
        if (RefreshToken == null) throw new ArgumentNullException(nameof(RefreshToken));

        using RedditWatcher watcher = new(AppId, AppSecret, RefreshToken);

        (string name, string description) = watcher.Start(Subreddit);
        Console.WriteLine($"Now watching {name}");
        Console.WriteLine(description);

        Console.WriteLine("Press ENTER to quit...");
        Console.ReadLine();
    }
}