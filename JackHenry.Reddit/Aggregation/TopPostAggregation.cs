namespace JackHenry.Reddit.Aggregation;

public class TopPostAggregation : IAggregation<PostSummary>, IFullObserver
{
    private readonly Dictionary<string, PostSummary> _tracker = new();

    public IFullObserver Observer => this;


    public event EventHandler<AggregationEventArgs<PostSummary>>? Updated;

    public void AcknowledgeItems(IEnumerable<PostSummary> items) => Update(items);

    public void UpdateItems(IEnumerable<PostSummary> items) => Update(items);

    public IEnumerable<PostSummary> GetResults()
    {
        return _tracker.Values.OrderByDescending(p => p.UpvoteCount);
    }


    private void Update(IEnumerable<PostSummary> items)
    {
        foreach (PostSummary item in items)
            Update(item);
        Updated?.Invoke(this, new AggregationEventArgs<PostSummary>(this));
    }

    private void Update(PostSummary item)
    {
        if (_tracker.ContainsKey(item.Id))
            _tracker[item.Id] = item;
        else
            _tracker.Add(item.Id, item);
    }
}
