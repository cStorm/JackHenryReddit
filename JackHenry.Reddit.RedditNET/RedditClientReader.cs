using JackHenry.Reddit.Api;
using Reddit;
using Reddit.Controllers;

namespace JackHenry.Reddit.RedditNET;

public class RedditClientReader : IRedditReader
{
    private readonly RedditClient _client;

    public RedditClientReader(ApiCredentials credentials)
    {
        _client = new(credentials.AppId, credentials.RefreshToken, credentials.AppSecret);
    }

    public SubredditSummary GetSubreddit(string name)
    {
        Subreddit sub = _client.Subreddit(name).About();
        return new(sub.Name, sub.Description);
    }


    private static PostSummary CreatePostSummary(Post post) => new(post.Author, post.Title, post.Id, post.UpVotes);

    public IIncrementalReader<PostSummary> GetLatestPosts(string subreddit, DateTime? oldest)
    {
        Subreddit sub = _client.Subreddit(subreddit);
        PageReader<Post, string> pager = new(after => sub?.Posts.GetNew(after),
                                             p => p.Fullname,
                                             p => p.Created < oldest);
        return new IncrementalPageReader<Post, PostSummary>(pager, CreatePostSummary);
    }

    void IDisposable.Dispose()
    {
    }
}
