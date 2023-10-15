namespace JackHenry.Reddit.Aggregation;

public class RedditAggregator : IRedditAggregator, IDisposable
{
    private readonly RedditWatcher _watcher;

    public RedditAggregator(RedditWatcher watcher)
    {
        _watcher = watcher ?? throw new ArgumentNullException(nameof(_watcher));
    }

    public void Include<T>(Filter filter, IAggregation<T> aggregation)
    {
        _watcher.Listen(filter, aggregation.Observer);
    }

    public void Dispose()
    {
        _watcher.Dispose();
    }
}
