using JackHenry.Reddit.Aggregation;
using JackHenry.Reddit.Api;
using JackHenry.Reddit.RedditNET;
using JackHenry.Reddit.Reporting;
using Microsoft.Extensions.DependencyInjection;

namespace JackHenry.Reddit.ConsoleApp;

public class DependencyInjector
{
    public IEnumerable<Type> GetImplementers<T>()
    {
        return GetType().Module.GetTypes().Where(t => t.IsAssignableTo(typeof(T)));
    }

    public void Inject(ServiceCollection services)
    {
        services.AddTransient<Authorizer>();
        services.AddTransient<IRedditReader, RedditClientReader>();
        services.AddTransient<IRedditMonitor, RedditMonitor>();
        //services.AddTransient<IRedditReader, RedditApiReader>();
        services.AddTransient<Provider<IRedditMonitor>>(sp => new(sp.GetRequiredService<IRedditMonitor>));

        services.AddTransient<IRedditAggregator, RedditAggregator>();

        services.AddTransient<RedditWatcher>();
        services.AddTransient<RedditService>();
    }

    public static void AddDefaultAggregations(RedditService s, Filter filter)
    {
        s.Aggregate<PostSummary, NewPostAggregation>(filter,
                  CreateReporter<PostSummary>((p, i) => $"New Post by {p.Username}: {p.Title}")
                  );
        s.Aggregate<UserHandle, TopUserAggregation>(filter,
                  CreateReporter<UserHandle>((u, i) => $"#{i + 1} {u.Username}"),
                  5);
        s.Aggregate<PostSummary, TopPostAggregation>(filter,
                  CreateReporter<PostSummary>((p, i) => $"#{i + 1} {p.UpvoteCount,5} - {p.Title}"),
                  3);
    }

    private static IAggregationReporter<T> CreateReporter<T>(Func<T, int, string> format)
    {
        return new TextAggregationReporter<T>(Console.Out, format);
    }
}
