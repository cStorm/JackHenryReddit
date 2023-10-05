using Microsoft.Extensions.DependencyInjection;

namespace JackHenry.Reddit.ConsoleApp;

public interface ICommand
{
    void Configure(ServiceCollection services);
    void Execute(ServiceProvider serviceProvider);
}
