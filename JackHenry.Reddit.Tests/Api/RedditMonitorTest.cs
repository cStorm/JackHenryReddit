using Moq;

namespace JackHenry.Reddit.Tests.Api;

public class RedditMonitorTest : IDisposable
{
    private static readonly TimeSpan _readTime = TimeSpan.FromMilliseconds(100);
    private readonly List<PostsEventArgs> _added = new();
    private readonly List<PostsEventArgs> _updated = new();
    private readonly Mock<IRedditReader> _reader = new();
    private readonly RedditMonitor _monitor;

    public RedditMonitorTest()
    {
        _monitor = new(_reader.Object);
        _monitor.PostsAdded += (sender, e) => _added.Add(e);
        _monitor.PostsUpdated += (sender, e) => _updated.Add(e);
    }

    public void Dispose() => _monitor.Dispose();


    [Fact]
    public void ReadsUntilDone()
    {
        Queue<IIncrementalReader<PostSummary>> readers = new(new[]{
            CreateIncrementalReader<PostSummary>(i => new("", "", $"{i}"), 1, 2),
            CreateIncrementalReader<PostSummary>(i => new("", "change", $"{i}"), 1, 1),
            CreateIncrementalReader<PostSummary>(i => new("", "", $"{i}"), 1, 1),
            CreateIncrementalReader<PostSummary>(i => new("", "", $"{i}"), 1, 1)
        });
        _reader.Setup(r => r.GetLatestPosts("sub", null)).Returns((string s, DateTime? d) =>
        {
            Thread.Sleep(_readTime);
            return readers.Dequeue();
        });

        _monitor.Start("sub");
        Thread.Sleep(_readTime * 1.5);
        Assert.Equal(2, _added.Count);
        Assert.Empty(_updated);

        Thread.Sleep(_readTime);
        Assert.Equal(2, _added.Count);
        Assert.Single(_updated);

        _monitor.Stop();
        Assert.Equal(2, _added.Count);
        Assert.Single(_updated);
        Assert.Single(readers);
    }

    [Fact]
    public void ReadsUntilStopped()
    {
        int count = 0;
        Queue<IIncrementalReader<PostSummary>> readers = new(Enumerable.Range(0, 3).Select(i =>
        {
            Mock<IIncrementalReader<PostSummary>> r = new();
            r.Setup(r => r.Enumerate()).Callback(() =>
            {
                count++;
                Thread.Sleep(_readTime);
            });
            return r.Object;
        }));
        _reader.Setup(r => r.GetLatestPosts("sub", null)).Returns((string s, DateTime? d) =>
        {
            return readers.Dequeue();
        });

        _monitor.Start("sub");
        Assert.Equal(0, count);
        Thread.Sleep(_readTime * .5);
        Assert.Equal(1, count);
        Thread.Sleep(_readTime);
        Assert.Equal(2, count);
        Thread.Sleep(_readTime);
        Assert.Equal(3, count);
        _monitor.Stop();
        Thread.Sleep(_readTime * 2);
        Assert.Equal(3, count);
        Assert.Equal(2, readers.Count);
    }

    [Fact]
    public void RunsUntilStopped()
    {
        Queue<IIncrementalReader<PostSummary>> readers = new(Enumerable.Range(0, 3).Select(i =>
        {
            return Mock.Of<IIncrementalReader<PostSummary>>(r => r.Done == true);
        }));
        _reader.Setup(r => r.GetLatestPosts("sub", null)).Returns((string s, DateTime? d) =>
        {
            Thread.Sleep(_readTime);
            return readers.Dequeue();
        });

        Thread.Sleep(_readTime * 2);
        _monitor.Start("sub");
        Assert.Equal(3, readers.Count);

        Thread.Sleep(_readTime * 1.5);
        Assert.Equal(2, readers.Count);

        _monitor.Stop();
        Assert.Single(readers);

        Thread.Sleep(_readTime * 2);
        Assert.Single(readers);
    }


    private static IIncrementalReader<T> CreateIncrementalReader<T>(Func<int, T> createItem, params int[] pageSizes)
    {
        return new TestIncrementalReader<T>(pageSizes.Select(size => Enumerable.Range(0, size).Select(createItem)));
    }

    private class TestIncrementalReader<T> : IIncrementalReader<T>
    {
        private readonly Queue<IEnumerable<T>> _queue;
        public TestIncrementalReader(IEnumerable<IEnumerable<T>> pages) => _queue = new(pages);

        public bool Done => _queue.Count == 0;
        public IEnumerable<T> Enumerate() => _queue.Dequeue();
    }
}
