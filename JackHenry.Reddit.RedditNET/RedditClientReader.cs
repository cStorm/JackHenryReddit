using JackHenry.Reddit.Api;
using Reddit;
using Reddit.Controllers;
using System.Diagnostics.CodeAnalysis;

namespace JackHenry.Reddit.RedditNET;

public class RedditClientReader : IRedditReader
{
    private readonly RedditClient _client;

    private Subreddit? _sub;
    private Thread? _updateThread;

    private readonly Dictionary<string, PostSummary> _posts = new();
    private DateTime? _oldest;

    public RedditClientReader(ApiCredentials credentials)
    {
        _client = new(credentials.AppId, credentials.RefreshToken, credentials.AppSecret);
    }

    public event EventHandler<PostsEventArgs>? PostsAdded;
    public event EventHandler<PostsEventArgs>? PostsUpdated;

    public SubredditSummary GetSubreddit(string name)
    {
        var _sub = _client.Subreddit(name).About();
        return new(_sub.Name, _sub.Description);
    }


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
        PageReader<Post, string> pager = new(after => _sub?.Posts.GetNew(after),
                                             p => p.Fullname,
                                             p => p.Created < _oldest);
        while (pager.ReadNext(out IEnumerable<Post>? posts))
        {
            List<PostSummary> changed = new();
            foreach (Post post in posts)
                if (UpdatePost(post, out PostSummary? summary))
                    changed.Add(summary);

            if (changed.Count != 0)
                PostsUpdated?.Invoke(this, new PostsEventArgs(changed));
        }
    }

    private bool UpdatePost(Post post, [MaybeNullWhen(false)] out PostSummary summary)
    {
        if (_posts.TryGetValue(post.Id, out summary))
        {
            var latest = CreatePostSummary(post);
            if (!summary.Equals(latest))
            {
                _posts[latest.Id] = latest;
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
