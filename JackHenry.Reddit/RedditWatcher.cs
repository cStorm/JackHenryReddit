namespace JackHenry.Reddit;

public class RedditWatcher : IDisposable
{
    private readonly IRedditReader _reader;
    private readonly IRedditMonitor _monitor;
    private readonly List<IFullObserver> _observers = new();

    public RedditWatcher(IRedditReader reader, IRedditMonitor monitor)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
    }

    public void Listen(IFullObserver observer)
    {
        _observers.Add(observer);
    }

    public SubredditSummary Start(string subName)
    {
        SubredditSummary sub = _reader.GetSubreddit(subName);

        _monitor.PostsAdded += (object? sender, PostsEventArgs e) =>
        {
            foreach (IRedditObserver<PostSummary> observer in _observers)
                observer.AcknowledgeItems(e.Items);
        };
        _monitor.PostsUpdated += (object? sender, PostsEventArgs e) =>
        {
            foreach (IRedditObserver<PostSummary> observer in _observers)
                observer.UpdateItems(e.Items);
        };
        _monitor.Start(subName);

        return sub;
    }

    public void Dispose()
    {
        _reader.Dispose();
        _monitor.Dispose();
    }
}
