namespace JackHenry.Reddit;

public class ChangeDistributor<T, TKey> where T : notnull where TKey : notnull
{
    private readonly Dictionary<TKey, T> _items = new();
    private readonly Func<T, TKey> _getKey;
    private readonly Action<IEnumerable<T>> _onAdded;
    private readonly Action<IEnumerable<T>> _onUpdated;

    public ChangeDistributor(Func<T, TKey> getKey, Action<IEnumerable<T>> onAdded, Action<IEnumerable<T>> onUpdated)
    {
        _getKey = getKey ?? throw new ArgumentNullException(nameof(getKey));
        _onAdded = onAdded ?? throw new ArgumentNullException(nameof(onAdded));
        _onUpdated = onUpdated ?? throw new ArgumentNullException(nameof(onUpdated));
    }

    public Task DistributeAsync(ICollection<T> items)
    {
        List<T> changed = new(), added = new();
        foreach (T item in items)
            if (UpdateItem(item, out bool existed))
                changed.Add(item);
            else if (!existed)
                added.Add(item);
        if (added.Count != 0)
            _onAdded(added);
        if (changed.Count != 0)
            _onUpdated(changed);
        return Task.CompletedTask;
    }

    private bool UpdateItem(T latest, out bool existed)
    {
        TKey key = _getKey(latest);
        if (_items.TryGetValue(key, out T? item))
        {
            existed = true;
            if (!item.Equals(latest))
            {
                _items[_getKey(latest)] = latest ?? throw new ArgumentNullException(nameof(latest));
                return true;
            }
        }
        else
        {
            existed = false;
            _items.Add(key, latest);
        }
        return false;
    }

    public void Clear()
    {
        _items.Clear();
    }
}
