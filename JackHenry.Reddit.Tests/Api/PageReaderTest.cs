using JackHenry.Reddit.Api;

namespace JackHenry.Reddit.Tests.Api;

public class PageReaderTest
{
    private List<Item>? _page;
    private PageReader<Item, int>? _reader;
    private int _after;


    [Fact]
    public void StopsOnEmpty()
    {
        _reader = CreateReader();
        _page = new() { new(1), new(2) };
        Assert.True(_reader.ReadNext(out var page));
        Assert.Equal(0, _after);
        Assert.Equal(_page, page);
        _page = new() { };
        Assert.False(_reader.ReadNext(out page));
        Assert.Equal(2, _after);
        Assert.Empty(page!);
    }

    [Fact]
    public void StopsOnCondition()
    {
        _reader = CreateReader(item => item.Id > 3);
        _page = new() { new(1), new(2) };
        Assert.True(_reader.ReadNext(out var page));
        Assert.Equal(0, _after);
        Assert.Equal(_page, page);
        _page = new() { new(3), new(4) };
        Assert.True(_reader.ReadNext(out page));
        Assert.Equal(2, _after);
        Assert.Single(page!);
        Assert.False(_reader.ReadNext(out page));
        Assert.Equal(2, _after);
        Assert.Null(page);
    }

    [Fact]
    public void StopsOnNull()
    {
        _reader = CreateReader();
        _page = new() { new(1), new(2) };
        Assert.True(_reader.ReadNext(out var page));
        Assert.Equal(0, _after);
        Assert.Equal(_page, page);
        _page = null;
        Assert.False(_reader.ReadNext(out page));
        Assert.Equal(2, _after);
        Assert.Null(page);
    }


    private PageReader<Item, int> CreateReader(Func<Item, bool>? stop = null)
    {
        _after = default;
        return new(after =>
        {
            _after = after;
            return _page;
        }, item => item.Id, stop);
    }

    private class Item
    {
        public Item(int id) => Id = id;
        public int Id { get; set; }
    }
}
