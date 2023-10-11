using JackHenry.Reddit.Reporting;

namespace JackHenry.Reddit.Aggregation;

public class RedditAggregator : IRedditAggregator, IDisposable
{
    private readonly RedditWatcher _watcher;

    public RedditAggregator(RedditWatcher watcher)
    {
        _watcher = watcher ?? throw new ArgumentNullException(nameof(_watcher));
    }

    public void Include<T>(IAggregation<T> aggregation, IAggregationReporter<T> reporter, int? count = null)
    {
        _watcher.Listen(aggregation.Observer);
        aggregation.Updated += (sender, args) => reporter.ReportAggregation(args.Aggregation, count);
    }

    public SubredditSummary Start(string subreddit)
    {
        return _watcher.Start(subreddit);
    }

    public void Dispose()
    {
        _watcher.Dispose();
    }
}
