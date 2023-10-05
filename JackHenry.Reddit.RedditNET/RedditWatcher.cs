using Reddit;
using Reddit.Controllers;
using Reddit.Controllers.EventArgs;

namespace JackHenry.Reddit.RedditNET;

public class RedditWatcher : IDisposable
{
    private readonly RedditClient _client;
    private Subreddit? _sub;

    public RedditWatcher(string appId, string? appSecret, string refreshToken)
    {
        _client = new(appId, refreshToken, appSecret);
    }

    public (string name, string description) Start(string subName)
    {
        if (_sub != null) throw new InvalidOperationException("Already running");

        _sub = _client.Subreddit(subName).About();

        UserDataTracker tracker = new();
        void C_NewPostsUpdated(object sender, PostsUpdateEventArgs e)
        {
            foreach (var post in e.Added)
            {
                var summary = new PostSummary(post.Author);
                tracker.AcknowledgePost(summary);
                Console.WriteLine("New Post by " + post.Author + ": " + post.Title);
            }
            int i = 0;
            foreach (var un in tracker.MostActive().Take(3))
                Console.WriteLine($"#{++i} {un}");
        }


        var posts = _sub.Posts.GetNew();
        Console.WriteLine(posts.Count);
        _sub.Posts.NewUpdated += C_NewPostsUpdated;

        if (!_sub.Posts.MonitorNew())
            throw new InvalidOperationException("Expected monitored");
        return (_sub.Name, _sub.Description);
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