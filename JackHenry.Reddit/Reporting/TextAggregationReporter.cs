namespace JackHenry.Reddit.Reporting;

public class TextAggregationReporter<T> : IAggregationReporter<T>
{
    private readonly TextWriter _writer;
    private readonly Func<T, int, string> _format;

    public TextAggregationReporter(TextWriter writer, Func<T, int, string> format)
    {
        _writer = writer ?? throw new ArgumentNullException(nameof(writer));
        _format = format ?? throw new ArgumentNullException(nameof(format));
    }

    public void ReportAggregation(IAggregation<T> aggregation, int? count = null)
    {
        Report(aggregation.GetResults(), count);
    }

    private void Report(IEnumerable<T> items, int? count = null)
    {
        if (count != null)
            items = items.Take(count.Value);
        foreach (string text in items.Select(_format))
            _writer.WriteLine(text);
    }
}
