namespace JackHenry.Reddit;

public interface IRedditReader : IDisposable
{
    event EventHandler<PostsEventArgs> PostsAdded;
    event EventHandler<PostsEventArgs> PostsUpdated;

    SubredditSummary GetSubreddit(string name);
    void Start(string name);
}
