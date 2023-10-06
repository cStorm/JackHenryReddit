namespace JackHenry.Reddit;

public class RedditWatcher : IDisposable
{
    private readonly IRedditReader _reader;

    public RedditWatcher(IRedditReader reader)
    {
        _reader = reader;
    }

    public event EventHandler<PostsEventArgs>? PostsAdded;
    public event EventHandler<UsersEventArgs>? ActiveUsersChanged;

    public SubredditSummary Start(string subName)
    {
        SubredditSummary sub = _reader.GetSubreddit(subName);

        UserDataTracker tracker = new();
        _reader.PostsAdded += (object? sender, PostsEventArgs e) =>
        {
            PostsAdded?.Invoke(this, e);
            foreach (PostSummary post in e.Items)
                tracker.AcknowledgePost(post);
            ActiveUsersChanged?.Invoke(this, new(tracker.MostActive()));
        };
        _reader.Start(subName);

        return sub;
    }

    public void Dispose()
    {
        _reader.Dispose();
    }
}
