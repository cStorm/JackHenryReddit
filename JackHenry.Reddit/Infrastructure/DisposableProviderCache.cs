using System.Collections.Concurrent;

namespace JackHenry.Reddit;

public class DisposableProviderCache<TKey, TService> : IDisposable
    where TKey : notnull
    where TService : IDisposable
{
    private readonly Provider<TService> _provider;
    private ConcurrentDictionary<TKey, TService>? _cache = new();
    public DisposableProviderCache(Provider<TService> provider) => _provider = provider ?? throw new ArgumentNullException(nameof(provider));

    protected ConcurrentDictionary<TKey, TService> Cache => _cache ?? throw new InvalidOperationException("Already disposed");

    public bool CreateOrGet(TKey key, out TService service)
    {
        bool created = false;
        service = Cache.GetOrAdd(key, k => { created = true; return _provider.Create(); });
        return created;
    }

    public void Dispose()
    {
        (IEnumerable<TService>? services, _cache) = (_cache?.Values, null);
        if (services != null)
            foreach (TService service in services)
                service.Dispose();
    }
}
