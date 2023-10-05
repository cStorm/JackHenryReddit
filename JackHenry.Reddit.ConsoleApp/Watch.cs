using CommandLine;
using JackHenry.Reddit.RedditNET;
using Microsoft.Extensions.DependencyInjection;

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

    public void Configure(ServiceCollection services)
    {
        if (AppId == null) throw new ArgumentNullException(nameof(AppId));
        if (RefreshToken == null) throw new ArgumentNullException(nameof(RefreshToken));

        services.AddTransient<RedditWatcher>(sp => new(AppId, AppSecret, RefreshToken));
    }

    public void Execute(ServiceProvider serviceProvider)
    {
        if (Subreddit == null) throw new ArgumentNullException(nameof(Subreddit));

        using RedditWatcher watcher = serviceProvider.GetRequiredService<RedditWatcher>();

        (string name, string description) = watcher.Start(Subreddit);
        Console.WriteLine($"Now watching {name}");
        Console.WriteLine(description);

        Console.WriteLine("Press ENTER to quit...");
        Console.ReadLine();
    }
}