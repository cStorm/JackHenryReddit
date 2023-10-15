using JackHenry.Reddit.ConsoleApp;
using JackHenry.Reddit.RedditNET;
using Microsoft.Extensions.DependencyInjection;

namespace JackHenry.Reddit.Tests.ConsoleApp;

public class DependencyInjectorTest
{
    private readonly DependencyInjector _di = new();

    [Theory]
    [InlineData(typeof(Authorizer))]
    [InlineData(typeof(RedditWatcher))]
    [InlineData(typeof(RedditService))]
    public void CanInjectServices(Type type)
    {
        ServiceCollection services = new();

        _di.Inject(services);
        services.AddSingleton(new OAuthRedirect());
        services.AddSingleton(new ApiCredentials("a", "s", "rt"));

        Assert.Single(services.BuildServiceProvider().GetServices(type));
    }
}
