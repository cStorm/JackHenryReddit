namespace JackHenry.Reddit.Tests.Aggregation;

public class NewPostAggregationTest
{
    private readonly NewPostAggregation _aggregation = new();
    private readonly PostSummary[] _items = Enumerable.Range(1, 5).Select(i => new PostSummary("u" + i, "t" + i)).ToArray();

    [Fact]
    public void DoesNotAggregate()
    {
        Assert.Empty(_aggregation.GetResults());
        _aggregation.Observer.AcknowledgeItems(_items.Take(3));
        Assert.Equal(_items.Take(3), _aggregation.GetResults());
        _aggregation.Observer.AcknowledgeItems(_items.Skip(3).Take(2));
        Assert.Equal(_items.Skip(3).Take(2), _aggregation.GetResults());
    }
}
