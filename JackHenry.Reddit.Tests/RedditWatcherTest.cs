using Moq;

namespace JackHenry.Reddit.Tests;

public class RedditWatcherTest : IDisposable
{
    private readonly List<Mock<IRedditMonitor>> _monitors = new();
    private readonly RedditWatcher _watcher;

    public RedditWatcherTest()
    {
        _watcher = new(new Provider<IRedditMonitor>(() =>
        {
            Mock<IRedditMonitor> monitor = new();
            _monitors.Add(monitor);
            return monitor.Object;
        }));
    }

    public void Dispose() => _watcher.Dispose();


    [Fact]
    public void WatchSubRaisesEvents()
    {
        const string name = "sub name";

        AddObserver(new(name, null), out var added, out var updated);

        var args = CreateArgs(new("a", "1"), new("b", "2"), new("a", "3"));
        // .Listen() currently auto-starts //_monitor.Raise(r => r.PostsAdded += null, args);
        Assert.Empty(added);
        Assert.Empty(updated);

        _watcher.Start();

        _monitors[0].Raise(r => r.PostsAdded += null, args);
        Assert.Equal(3, added.Count);
        Assert.Empty(updated);

        _monitors[0].Raise(r => r.PostsAdded += null, CreateArgs(new("c", "4"), new("a", "5")));
        _monitors[0].Raise(r => r.PostsUpdated += null, args);
        Assert.Equal(5, added.Count);
        Assert.Equal(3, updated.Count);
    }

    [Fact]
    public void MonitorIsReused()
    {
        const string name = "sub name";
        AddObserver(new(name, null), out var added1, out var updated1);
        AddObserver(new(name, null), out var added2, out var updated2);
        Assert.Single(_monitors);
        _watcher.Start();

        _monitors[0].Raise(r => r.PostsAdded += null, CreateArgs(new PostSummary("c3", "1")));
        _monitors[0].Raise(r => r.PostsUpdated += null, CreateArgs(new("a", "1"), new("b", "2")));
        Assert.Single(added1);
        Assert.Equal(2, updated1.Count);
        Assert.Single(added2);
        Assert.Equal(2, updated2.Count);
    }

    [Fact]
    public void ObserversAreSeparate()
    {
        AddObserver(new("sub 1", null), out var added1, out var updated1);
        AddObserver(new("sub 2", null), out var added2, out var updated2);
        Assert.Equal(2, _monitors.Count);
        _watcher.Start();

        _monitors[0].Raise(r => r.PostsAdded += null, CreateArgs(new PostSummary("c3", "1")));
        _monitors[1].Raise(r => r.PostsUpdated += null, CreateArgs(new("a", "1"), new("b", "2")));
        Assert.Single(added1);
        Assert.Empty(updated1);
        Assert.Empty(added2);
        Assert.Equal(2, updated2.Count);
    }


    private void AddObserver(Filter filter, out List<PostSummary> added, out List<PostSummary> updated)
    {
        added = new();
        updated = new();
        var observer = new Mock<IFullObserver>();
        observer.Setup(o => o.AcknowledgeItems(It.IsAny<IEnumerable<PostSummary>>())).Callback(added.AddRange);
        observer.Setup(o => o.UpdateItems(It.IsAny<IEnumerable<PostSummary>>())).Callback(updated.AddRange);
        _watcher.Listen(filter, observer.Object);
    }

    private static PostsEventArgs CreateArgs(params PostSummary[] items)
    {
        return new PostsEventArgs(items);
    }
}
