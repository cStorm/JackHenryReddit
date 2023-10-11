using JackHenry.Reddit.Aggregation;

namespace JackHenry.Reddit.Tests.Aggregation;

public class TopPostAggregationTest
{
    private readonly TopPostAggregation _aggregation = new();

    [Fact]
    public void OrderChanges()
    {
        Assert.Empty(_aggregation.GetResults());
        _aggregation.Observer.AcknowledgeItems(CreatePosts(5, 9, 1));
        AssertIds(1, 0, 2);
        _aggregation.Observer.UpdateItems(CreatePosts(0, 0, 7, 11).Skip(2));
        AssertIds(3, 1, 2, 0);
    }

    private static IEnumerable<PostSummary> CreatePosts(params int[] ups)
    {
        return ups.Select((u, i) => new PostSummary("", "", $"{i}", u));
    }

    private void AssertIds(params int[] ids)
    {
        Assert.Equal(ids.Select(id => $"{id}"), _aggregation.GetResults().Select(p => p.Id));
    }
}
