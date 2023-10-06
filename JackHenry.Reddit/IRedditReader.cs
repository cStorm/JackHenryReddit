namespace JackHenry.Reddit;

public interface IRedditReader : IDisposable
{
    event EventHandler<PostsEventArgs> PostsAdded;

    SubredditSummary GetSubreddit(string name);
    void Start(string name);
}
