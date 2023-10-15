using CommandLine;
using JackHenry.Reddit.Aggregation;
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

    [Option('r', "refresh", MetaValue = "refresh-token", Required = true)]
    public string? RefreshToken { get; set; }

    public void Configure(ServiceCollection services)
    {
        if (AppId == null) throw new ArgumentNullException(nameof(AppId));
        if (RefreshToken == null) throw new ArgumentNullException(nameof(RefreshToken));

        services.AddSingleton(new ApiCredentials(AppId, AppSecret, RefreshToken));
    }

    public void Execute(ServiceProvider serviceProvider)
    {
        if (Subreddit == null) throw new ArgumentNullException(nameof(Subreddit));

        var service = serviceProvider.GetRequiredService<RedditService>();
        SubredditSummary subreddit = service.GetSubreddit(Subreddit);
        {
            var filter = new Filter(Subreddit, null);
            DependencyInjector.AddDefaultAggregations(service, filter);
        }

        Console.WriteLine($"Now watching {subreddit.Name}");
        Console.WriteLine(subreddit.Description);

        Console.WriteLine("Press ENTER to quit...");
        Console.ReadLine();
    }
}