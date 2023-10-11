using JackHenry.Reddit.Aggregation;
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

        services.AddTransient<RedditAggregator>();
        services.AddTransient<IRedditAggregator>(sp =>
        {
            var s = sp.GetRequiredService<RedditAggregator>();
            s.Include(new NewPostAggregation(),
                      CreateReporter<PostSummary>((p, i) => $"New Post by {p.Username}: {p.Title}"));
            s.Include(new TopUserAggregation(),
                      CreateReporter<UserHandle>((u, i) => $"#{i + 1} {u.Username}"),
                      5);
            s.Include(new TopPostAggregation(),
                      CreateReporter<PostSummary>((p, i) => $"#{i + 1} {p.UpvoteCount,5} - {p.Title}"),
                      3);
            return s;
        });

        services.AddTransient<RedditWatcher>();
    }

    private static IAggregationReporter<T> CreateReporter<T>(Func<T, int, string> format)
    {
        return new TextAggregationReporter<T>(Console.Out, format);
    }
}
