using Moq;

namespace JackHenry.Reddit.Tests;

public class RedditWatcherTest : IDisposable
{
    private readonly Mock<IRedditReader> _reader;
    private readonly RedditWatcher _watcher;

    public RedditWatcherTest()
    {
        _reader = new();
        _watcher = new(_reader.Object);
    }

    public void Dispose() => _watcher.Dispose();

    [Fact]
    public void WatchSubRaisesEvents()
    {
        const string name = "sub name";
        SubredditSummary sub = new(name, "desc");

        _reader.Setup(r => r.GetSubreddit(name)).Returns(sub);

        List<PostSummary> added = new(), updated = new();
        var observer = new Mock<IFullObserver>();
        observer.Setup(o => o.AcknowledgeItems(It.IsAny<IEnumerable<PostSummary>>())).Callback(added.AddRange);
        observer.Setup(o => o.UpdateItems(It.IsAny<IEnumerable<PostSummary>>())).Callback(updated.AddRange);
        _watcher.Listen(observer.Object);

        var args = CreateArgs(new("a", "1"), new("b", "2"), new("a", "3"));
        _reader.Raise(r => r.PostsAdded += null, args);
        Assert.Empty(added);
        Assert.Empty(updated);

        Assert.Equal(sub, _watcher.Start(name));

        _reader.Raise(r => r.PostsAdded += null, args);
        Assert.Equal(3, added.Count);
        Assert.Empty(updated);

        _reader.Raise(r => r.PostsAdded += null, CreateArgs(new("c", "4"), new("a", "5")));
        _reader.Raise(r => r.PostsUpdated += null, args);
        Assert.Equal(5, added.Count);
        Assert.Equal(3, updated.Count);
    }

    private static PostsEventArgs CreateArgs(params PostSummary[] items)
    {
        return new PostsEventArgs(items);
    }
}
