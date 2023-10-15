namespace JackHenry.Reddit;

public class RedditMonitor : IRedditMonitor
{
    private readonly IRedditReader _redditReader;

    private CancellationTokenSource? _updateThread;
    private Task? _task;

    private readonly ChangeDistributor<PostSummary, string> _distributor;

    public RedditMonitor(IRedditReader redditReader)
    {
        _redditReader = redditReader ?? throw new ArgumentNullException(nameof(redditReader));
        _distributor = new(
            post => post.Id,
            posts => PostsAdded?.Invoke(this, new PostsEventArgs(posts)),
            posts => PostsUpdated?.Invoke(this, new PostsEventArgs(posts))
            );
    }

    public event EventHandler<PostsEventArgs>? PostsAdded;
    public event EventHandler<PostsEventArgs>? PostsUpdated;

    public void Start(string subName, DateTime? oldest = null)
    {
        if (_updateThread != null) throw new InvalidOperationException("Already running");

        _updateThread = new();
        _task = MonitorPostsAsync(subName, oldest, _updateThread.Token);
    }

    private async Task MonitorPostsAsync(string subreddit, DateTime? oldest, CancellationToken cancellationToken)
    {
        await Task.Yield();
        while (!cancellationToken.IsCancellationRequested)
            try
            {
                await CheckPostsAsync(subreddit, oldest, cancellationToken);
            }
            catch { }
    }

    private async Task CheckPostsAsync(string subreddit, DateTime? oldest, CancellationToken cancellationToken)
    {
        List<Task> tasks = new();

        IIncrementalReader<PostSummary> incremental = _redditReader.GetLatestPosts(subreddit, oldest);
        while (!incremental.Done && !cancellationToken.IsCancellationRequested)
        {
            List<PostSummary> posts = incremental.Enumerate().ToList();
            Task distribute = _distributor.DistributeAsync(posts);
            tasks.Add(distribute);
        }

        await Task.WhenAll(tasks);
    }

    public void Stop()
    {
        if (_updateThread == null) throw new InvalidOperationException("Not running");

        using CancellationTokenSource cancellation = _updateThread;
        cancellation.Cancel();
        _task?.Wait();
        _distributor.Clear();
        _updateThread = null;
    }

    public void Dispose()
    {
        if (_updateThread != null)
            Stop();
    }
}
