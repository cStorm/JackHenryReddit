using CommandLine;
using JackHenry.Reddit.ConsoleApp;
using Microsoft.Extensions.DependencyInjection;

DependencyInjector di = new();

ServiceCollection services = new();
di.Inject(services);

Parser.Default.ParseArguments(args, di.GetImplementers<ICommand>().ToArray())
    .WithParsed<ICommand>(c => ExecuteCommand(c, services));

static void ExecuteCommand(ICommand command, ServiceCollection services)
{
    command.Configure(services);

    using ServiceProvider serviceProvider = services.BuildServiceProvider();
    command.Execute(serviceProvider);
}
