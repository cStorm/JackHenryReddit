﻿using JackHenry.Reddit.Api.Contracts;
using JackHenry.Reddit.Api.Http;

namespace JackHenry.Reddit.Api;

public class RedditApiReader : IRedditReader
{
    private readonly ApiClient _apiClient;
    private readonly int _limit = 100;

    public RedditApiReader(ApiCredentials credentials)
    {
        _apiClient = new HttpApiClient(credentials, new Throttle());
        //_apiClient = new RestSharpApiClient(credentials);
    }

    public SubredditSummary GetSubreddit(string name)
    {
        var task = _apiClient.GetAsync<Container<SubredditData>>("r/" + name + "/about");
        Container<SubredditData> result = task.Result;
        return new SubredditSummary(name, result.data.public_description);
    }

    public IIncrementalReader<PostSummary> GetLatestPosts(string subreddit, DateTime? oldest)
    {
        return GetLatestPosts(subreddit, oldest, default);
    }
    public IIncrementalReader<PostSummary> GetLatestPosts(string subreddit, DateTime? oldest, CancellationToken cancellationToken = default)
    {
        PageReader<PostData, string> pager = new(
            after =>
            {
                Task<IEnumerable<PostData>> task = GetListingAsync<PostData>($"r/{subreddit}/new?after={after}&limit={_limit}", cancellationToken);
                return task.Result;
            },
            p => p.name,
            p => cancellationToken.IsCancellationRequested || p.GetCreated() < oldest);
        return new IncrementalPageReader<PostData, PostSummary>(pager, p => p.CreatePostSummary());
    }

    private async Task<IEnumerable<T>> GetListingAsync<T>(string url, CancellationToken cancellationToken = default)
    {
        var container = await _apiClient.GetAsync<Container<Listing<Container<T>>>>(url, cancellationToken);
        return container.data.children.Select(c => c.data);
    }

    public void Dispose()
    {
        (_apiClient as IDisposable)?.Dispose();
    }
}
