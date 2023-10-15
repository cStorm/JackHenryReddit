namespace JackHenry.Reddit;

public class RedditWatcher : IDisposable
{
    private readonly DisposableProviderCache<Filter, IRedditMonitor> _monitorProvider;

    public RedditWatcher(Provider<IRedditMonitor> monitorProvider)
    {
        _monitorProvider = new(monitorProvider);
    }

    public void Listen(Filter filter, IFullObserver observer)
    {
        bool created = _monitorProvider.CreateOrGet(filter, out var monitor);
        //_observers.Add(observer);
        monitor.PostsAdded += (object? sender, PostsEventArgs e) => observer.AcknowledgeItems(e.Items);
        monitor.PostsUpdated += (object? sender, PostsEventArgs e) => observer.UpdateItems(e.Items);
        if (created)
            monitor.Start(filter.Subreddit, filter.Oldest);
    }

    public void Start()
    {
        // for now try just letting it auto-start
    }

    public void Dispose()
    {
        _monitorProvider.Dispose();
    }
}
