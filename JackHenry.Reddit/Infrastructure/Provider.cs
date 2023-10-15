namespace JackHenry.Reddit;

public class Provider<T>
{
    private readonly Func<T> _create;
    public Provider(Func<T> create) => _create = create ?? throw new ArgumentNullException(nameof(create));
    public T Create() => _create();
}
