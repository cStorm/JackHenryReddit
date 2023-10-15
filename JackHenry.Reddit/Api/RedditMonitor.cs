namespace JackHenry.Reddit;

public class RedditMonitor : IRedditMonitor
{
    private readonly IRedditReader _redditReader;

    private string? _subreddit;
    private DateTime? _oldest;
    private Thread? _updateThread;

    private readonly Dictionary<string, PostSummary> _posts = new();

    public RedditMonitor(IRedditReader redditReader)
    {
        _redditReader = redditReader ?? throw new ArgumentNullException(nameof(redditReader));
    }

    public event EventHandler<PostsEventArgs>? PostsAdded;
    public event EventHandler<PostsEventArgs>? PostsUpdated;

    public void Start(string subName, DateTime? oldest = null)
    {
        if (_updateThread != null) throw new InvalidOperationException("Already running");

        _subreddit = subName;
        _oldest = oldest;
        MonitorPosts();
    }

    private void MonitorPosts()
    {
        _updateThread = new Thread(() =>
        {
            while (_updateThread != null)
                try
                {
                    CheckPosts();
                }
                catch { }
        });
        _updateThread.Start();
    }

    private void CheckPosts()
    {
        (string? subreddit, DateTime? oldest) = (_subreddit, _oldest);
        if (subreddit != null)
        {
            IIncrementalReader<PostSummary> incremental = _redditReader.GetLatestPosts(subreddit, _oldest);
            while (!incremental.Done && _updateThread != null)
            {
                List<PostSummary> changed = new(), added = new();
                foreach (PostSummary summary in incremental.Enumerate())
                    if (UpdatePost(summary, out bool existed))
                        changed.Add(summary);
                    else if (!existed)
                        added.Add(summary);
                if (added.Count != 0)
                    PostsAdded?.Invoke(this, new PostsEventArgs(added));
                if (changed.Count != 0)
                    PostsUpdated?.Invoke(this, new PostsEventArgs(changed));
            }
        }
    }

    private bool UpdatePost(PostSummary latest, out bool existed)
    {
        if (_posts.TryGetValue(latest.Id, out PostSummary? summary))
        {
            existed = true;
            if (!summary.Equals(latest))
            {
                _posts[latest.Id] = latest ?? throw new ArgumentNullException(nameof(latest));
                return true;
            }
        }
        else
        {
            existed = false;
            _posts.Add(latest.Id, latest);
        }
        return false;
    }

    public void Stop()
    {
        if (_updateThread == null) throw new InvalidOperationException("Not running");

        (Thread? thread, _updateThread) = (_updateThread, null);
        (_subreddit, _oldest) = (null, null);
        thread?.Join();
        _posts.Clear();
    }

    public void Dispose()
    {
        if (_updateThread != null)
            Stop();
    }
}
