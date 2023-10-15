namespace JackHenry.Reddit.Aggregation;

public interface IRedditAggregator
{
    void Include<T>(Filter filter, IAggregation<T> aggregation);
}
