namespace JackHenry.Reddit.Tests.Aggregation;

public class TopUserAggregationTest
{
    private readonly TopUserAggregation _aggregation = new();

    [Fact]
    public void OrderChanges()
    {
        Assert.Empty(_aggregation.GetResults());
        _aggregation.Observer.AcknowledgeItems(CreatePosts("a", "b", "a"));
        AssertNames("a", "b");
        _aggregation.Observer.AcknowledgeItems(CreatePosts("c", "b", "c", "b", "c", "b"));
        AssertNames("b", "c", "a");
    }

    private static IEnumerable<PostSummary> CreatePosts(params string[] names)
    {
        foreach (string name in names)
            yield return new PostSummary(name, $"{Guid.NewGuid()}");
    }

    private void AssertNames(params string[] names)
    {
        Assert.Equal(names, _aggregation.GetResults().Select(u => u.Username));
    }
}
