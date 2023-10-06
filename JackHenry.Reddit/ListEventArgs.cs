namespace JackHenry.Reddit;

public class ListEventArgs<T> : EventArgs
{
    public ListEventArgs(IEnumerable<T> items) => Items = items.ToList();

    public IList<T> Items { get; }
}
