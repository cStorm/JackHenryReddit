namespace JackHenry.Reddit.Reporting;

public interface IAggregationReporter<T>
{
    void ReportAggregation(IAggregation<T> aggregation, int? count = null);
}
