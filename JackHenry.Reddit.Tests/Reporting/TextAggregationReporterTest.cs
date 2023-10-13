using JackHenry.Reddit.Aggregation;
using JackHenry.Reddit.Reporting;
using Moq;

namespace JackHenry.Reddit.Tests.Reporting;

public class TextAggregationReporterTest
{
    private StringWriter _writer = new() { NewLine = "\n" };
    private Mock<IAggregation<int>> _aggregation = new Mock<IAggregation<int>>();

    [Fact]
    public void ReportAll()
    {
        var reporter = new TextAggregationReporter<int>(_writer, (o, i) => $"a{o}z");
        _aggregation.Setup(a => a.GetResults()).Returns(Enumerable.Range(3, 3));
        reporter.ReportAggregation(_aggregation.Object);
        AssertWriter(@"a3z
a4z
a5z
");
    }

    [Fact]
    public void ReportLimit()
    {
        var reporter = new TextAggregationReporter<int>(_writer, (o, i) => $"a{o}z");
        _aggregation.Setup(a => a.GetResults()).Returns(Enumerable.Range(3, 2));
        reporter.ReportAggregation(_aggregation.Object);
        AssertWriter(@"a3z
a4z
");
    }

    [Fact]
    public void ReportWithIndex()
    {
        var reporter = new TextAggregationReporter<int>(_writer, (o, i) => $"a{i}:{o}z");
        _aggregation.Setup(a => a.GetResults()).Returns(Enumerable.Range(3, 2));
        reporter.ReportAggregation(_aggregation.Object);
        AssertWriter(@"a0:3z
a1:4z
");
    }

    void AssertWriter(string text)
    {
        Assert.Equal(text.Replace("\r", ""), _writer.ToString());
    }
}
