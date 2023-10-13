namespace JackHenry.Reddit.Aggregation;

public class AggregationEventArgs<T> : EventArgs
{
    public AggregationEventArgs(IAggregation<T> aggregation)
    {
        Aggregation = aggregation ?? throw new ArgumentNullException(nameof(aggregation));
    }

    public IAggregation<T> Aggregation { get; }
}
