namespace JackHenry.Reddit;

public interface IRedditReader : IDisposable
{
    SubredditSummary GetSubreddit(string name);
    IIncrementalReader<PostSummary> GetLatestPosts(string subreddit, DateTime? oldest);
}
