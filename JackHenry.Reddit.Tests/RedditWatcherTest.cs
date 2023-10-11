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

        List<PostSummary> added = new();
        var observer = new Mock<IFullObserver>();
        observer.Setup(o => o.AcknowledgeItems(It.IsAny<IEnumerable<PostSummary>>())).Callback(added.AddRange);
        _watcher.Listen(observer.Object);

        var args = new PostsEventArgs(new PostSummary[] { new("a", "1"), new("b", "2"), new("a", "3") });
        _reader.Raise(r => r.PostsAdded += null, args);
        Assert.Empty(added);

        Assert.Equal(sub, _watcher.Start(name));

        _reader.Raise(r => r.PostsAdded += null, args);
        Assert.Equal(3, added.Count);

        args = new PostsEventArgs(new PostSummary[] { new("c", "4"), new("a", "5") });
        _reader.Raise(r => r.PostsAdded += null, args);
        Assert.Equal(5, added.Count);
    }
}
