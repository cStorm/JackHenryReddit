using JackHenry.Reddit.Aggregation;
using JackHenry.Reddit.Reporting;

namespace JackHenry.Reddit;

public class RedditService : IDisposable
{
    private readonly IRedditReader _reader;
    private readonly IRedditAggregator _aggregator;

    public RedditService(IRedditReader reader, IRedditAggregator aggregator)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _aggregator = aggregator ?? throw new ArgumentNullException(nameof(aggregator));
    }


    public SubredditSummary GetSubreddit(string name)
    {
        return _reader.GetSubreddit(name);
    }


    public void Aggregate<T, TAgg>(Filter filter, IAggregationReporter<T> reporter, int? count = null) where TAgg : IAggregation<T>, new()
    {
        var aggregation = new TAgg();
        aggregation.Updated += (sender, args) => reporter.ReportAggregation(args.Aggregation, count);
        _aggregator.Include(filter, aggregation);
    }


    public void Dispose()
    {
        _reader.Dispose();
        (_aggregator as IDisposable)?.Dispose();
    }
}
