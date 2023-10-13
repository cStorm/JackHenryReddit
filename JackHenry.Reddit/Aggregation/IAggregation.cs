namespace JackHenry.Reddit.Aggregation;

public interface IAggregation
{
    IFullObserver Observer { get; }
}

public interface IAggregation<T> : IAggregation
{
    event EventHandler<AggregationEventArgs<T>> Updated;
    IEnumerable<T> GetResults();
}
