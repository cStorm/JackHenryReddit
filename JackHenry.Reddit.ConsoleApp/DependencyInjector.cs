using Microsoft.Extensions.DependencyInjection;

namespace JackHenry.Reddit.ConsoleApp;

public class DependencyInjector
{
    public IEnumerable<Type> GetImplementers<T>()
    {
        return GetType().Module.GetTypes().Where(t => t.IsAssignableTo(typeof(T)));
    }

    public void Inject(ServiceCollection services)
    {
        services.AddTransient<RedditWatcher>();
    }
}
