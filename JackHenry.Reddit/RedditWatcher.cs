namespace JackHenry.Reddit;

public class RedditWatcher : IDisposable
{
    private readonly IRedditReader _reader;
    private readonly List<IFullObserver> _observers = new();

    public RedditWatcher(IRedditReader reader)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
    }

    public void Listen(IFullObserver observer)
    {
        _observers.Add(observer);
    }

    public SubredditSummary Start(string subName)
    {
        SubredditSummary sub = _reader.GetSubreddit(subName);

        _reader.PostsAdded += (object? sender, PostsEventArgs e) =>
        {
            foreach (IRedditObserver<PostSummary> observer in _observers)
                observer.AcknowledgeItems(e.Items);
        };
        _reader.Start(subName);

        return sub;
    }

    public void Dispose()
    {
        _reader.Dispose();
    }
}
