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

        List<PostsEventArgs> added = new();
        List<UsersEventArgs> users = new();
        _watcher.PostsAdded += (sender, e) => added.Add(e);
        _watcher.ActiveUsersChanged += (sender, e) => users.Add(e);

        var args = new PostsEventArgs(new PostSummary[] { new("a", "1"), new("b", "2"), new("a", "3") });
        _reader.Raise(r => r.PostsAdded += null, args);
        Assert.Empty(added);
        Assert.Empty(users);

        Assert.Equal(sub, _watcher.Start(name));

        _reader.Raise(r => r.PostsAdded += null, args);
        Assert.Equal(3, added.Last().Items.Count());
        Assert.Equal(2, users.Last().Items.Count());

        args = new PostsEventArgs(new PostSummary[] { new("c", "4"), new("a", "5") });
        _reader.Raise(r => r.PostsAdded += null, args);
        Assert.Equal(2, added.Last().Items.Count());
        Assert.Equal(3, users.Last().Items.Count());
    }
}
