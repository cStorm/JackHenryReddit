using Reddit;
using Reddit.Controllers;

namespace JackHenry.Reddit.RedditNET;

public class RedditClientReader : IRedditReader
{
    private readonly RedditClient _client;
    private Subreddit? _sub;

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
            HashSet<Post> added = new(e.Added);
            PostsAdded?.Invoke(this, new PostsEventArgs(added.Select(CreatePostSummary)));
            PostsUpdated?.Invoke(this, new PostsEventArgs(e.NewPosts.Where(p => !added.Contains(p)).Select(CreatePostSummary)));
        };
        if (!_sub.Posts.MonitorNew())
            throw new InvalidOperationException("Expected monitored");
    }


    public void Stop()
    {
        if (_sub == null) throw new InvalidOperationException("Not running");

        (Subreddit sub, _sub) = (_sub, null);
        if (sub.Posts.MonitorNew())
            throw new InvalidOperationException("Expected unmonitored");
    }

    public void Dispose()
    {
        if (_sub != null)
            Stop();
    }
}
