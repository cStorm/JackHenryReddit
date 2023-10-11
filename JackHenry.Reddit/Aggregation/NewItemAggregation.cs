namespace JackHenry.Reddit;

public abstract class NewItemAggregation<T> : IAggregation<T>, IFullObserver
{
    private IEnumerable<T> _latest = Enumerable.Empty<T>();

    public IFullObserver Observer => this;

    public event EventHandler<AggregationEventArgs<T>>? Updated;

    public abstract void AcknowledgeItems(IEnumerable<T> items);
    protected void Added(IEnumerable<T> items)
    {
        _latest = items;
        Updated?.Invoke(this, new AggregationEventArgs<T>(this));
    }

    public IEnumerable<T> GetResults()
    {
        return _latest;
    }
}
