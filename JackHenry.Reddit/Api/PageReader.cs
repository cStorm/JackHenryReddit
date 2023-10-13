using System.Diagnostics.CodeAnalysis;

namespace JackHenry.Reddit.Api;

public class PageReader<T, TNext> : IPageReader<T>
{
    private readonly Func<TNext?, IEnumerable<T>?> _readPage;
    private readonly Func<T, TNext> _getToken;
    private readonly Func<T, bool>? _stop;
    private TNext? _continuation;
    private bool _continue = true;

    public PageReader(Func<TNext?, IEnumerable<T>?> readPage, Func<T, TNext> getToken, Func<T, bool>? stop = null)
    {
        _readPage = readPage ?? throw new ArgumentNullException(nameof(readPage));
        _getToken = getToken ?? throw new ArgumentNullException(nameof(getToken));
        _stop = stop;
    }

    public bool ReadNext([MaybeNullWhen(false)] out IEnumerable<T> page)
    {
        if (_continue)
        {
            page = _readPage(_continuation);
            if (page == null)
                return _continue = false;

            IEnumerator<T> enumerator = page.GetEnumerator();
            bool any = enumerator.MoveNext();
            if (any)
                page = ReadPage(enumerator);
            return any;
        }
        page = null;
        return false;
    }
    private IEnumerable<T> ReadPage(IEnumerator<T> page)
    {
        do
        {
            T item = page.Current;
            _continuation = _getToken(item);
            if (_stop?.Invoke(item) == true)
            {
                _continue = false;
                break;
            }
            yield return item;
        } while (page.MoveNext());
    }
}
