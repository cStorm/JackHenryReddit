namespace JackHenry.Reddit.Api;

public class IncrementalPageReader<TRecord, T> : IIncrementalReader<T>
{
    private readonly IPageReader<TRecord> _pageReader;
    private readonly Func<TRecord, T> _readItem;

    public IncrementalPageReader(IPageReader<TRecord> pageReader, Func<TRecord, T> readItem)
    {
        _pageReader = pageReader ?? throw new ArgumentNullException(nameof(pageReader));
        _readItem = readItem;
    }

    public bool Done { get; private set; }

    public IEnumerable<T> Enumerate()
    {
        if (_pageReader.ReadNext(out IEnumerable<TRecord>? page))
            return page.Select(_readItem);
        Done = true;
        return Enumerable.Empty<T>();
    }
}
