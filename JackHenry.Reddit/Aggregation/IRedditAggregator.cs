using JackHenry.Reddit.Reporting;

namespace JackHenry.Reddit;

public interface IRedditAggregator
{
    void Include<T>(IAggregation<T> aggregation, IAggregationReporter<T> reporter, int? count = null);
    SubredditSummary Start(string subreddit);
}
