namespace JackHenry.Reddit;

public interface IRedditMonitor : IDisposable
{
    event EventHandler<PostsEventArgs> PostsAdded;
    event EventHandler<PostsEventArgs> PostsUpdated;

    void Start(string subreddit, DateTime? oldest = null);
    void Stop();
}
