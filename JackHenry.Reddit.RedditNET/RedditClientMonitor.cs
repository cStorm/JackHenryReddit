using Reddit;
using Reddit.Controllers;

namespace JackHenry.Reddit.RedditNET;

public class RedditClientMonitor : IRedditMonitor
{
    private readonly RedditClient _client;
    private readonly IRedditReader _redditReader;

    private Subreddit? _sub;
    private Thread? _updateThread;

    private readonly Dictionary<string, PostSummary> _posts = new();
    private DateTime? _oldest;

    public RedditClientMonitor(ApiCredentials credentials, IRedditReader redditReader)
    {
        _client = new(credentials.AppId, credentials.RefreshToken, credentials.AppSecret);
        _redditReader = redditReader ?? throw new ArgumentNullException(nameof(redditReader));
    }

    public event EventHandler<PostsEventArgs>? PostsAdded;
    public event EventHandler<PostsEventArgs>? PostsUpdated;

    private static PostSummary CreatePostSummary(Post post) => new(post.Author, post.Title, post.Id, post.UpVotes);

    public void Start(string subName)
    {
        if (_sub != null) throw new InvalidOperationException("Already running");

        _sub = _client.Subreddit(subName).About();
        _sub.Posts.NewUpdated += (sender, e) =>
        {
            var added = e.Added.Select(AddPost).ToArray();
            PostsAdded?.Invoke(this, new PostsEventArgs(added));
        };
        if (!_sub.Posts.MonitorNew())
            throw new InvalidOperationException("Expected monitored");

        MonitorPosts();
    }

    private PostSummary AddPost(Post post)
    {
        PostSummary summary = CreatePostSummary(post);
        _posts.Add(summary.Id, summary);
        if (_oldest > post.Created || _oldest == null)
            _oldest = post.Created;
        return summary;
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
        string? subreddit = _sub?.Name;
        if (subreddit != null)
        {
            IIncrementalReader<PostSummary> incremental = _redditReader.GetLatestPosts(subreddit, _oldest);
            while (!incremental.Done)
            {
                List<PostSummary> changed = incremental.Enumerate().Where(UpdatePost).ToList();
                if (changed.Count != 0)
                    PostsUpdated?.Invoke(this, new PostsEventArgs(changed));
            }
        }
    }

    private bool UpdatePost(PostSummary latest)
    {
        if (_posts.TryGetValue(latest.Id, out PostSummary? summary))
        {
            if (!summary.Equals(latest))
            {
                _posts[latest.Id] = latest ?? throw new ArgumentNullException(nameof(latest));
                return true;
            }
        }
        return false;
    }

    public void Stop()
    {
        if (_sub == null) throw new InvalidOperationException("Not running");

        (Subreddit sub, _sub) = (_sub, null);
        (Thread? thread, _updateThread) = (_updateThread, null);
        bool monitored = sub.Posts.MonitorNew();
        thread?.Join();
        _posts.Clear();
        _oldest = null;

        if (monitored)
            throw new InvalidOperationException("Expected unmonitored");
    }

    public void Dispose()
    {
        if (_sub != null)
            Stop();
    }
}
