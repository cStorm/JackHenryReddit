namespace JackHenry.Reddit;

public class TopUserAggregation : IAggregation<UserHandle>, IFullObserver
{
    private readonly UserDataTracker _tracker = new();

    public IFullObserver Observer => this;

    public event EventHandler<AggregationEventArgs<UserHandle>>? Updated;

    public void AcknowledgeItems(IEnumerable<PostSummary> items)
    {
        foreach (PostSummary item in items)
            _tracker.AcknowledgePost(item);
        Updated?.Invoke(this, new AggregationEventArgs<UserHandle>(this));
    }

    public IEnumerable<UserHandle> GetResults()
    {
        return _tracker.MostActive().Select(u => new UserHandle(u));
    }
}
