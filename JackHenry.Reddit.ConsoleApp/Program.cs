using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using ICommand = JackHenry.Reddit.ConsoleApp.ICommand;

ServiceCollection services = new();

Parser.Default.ParseArguments(args, GetImplementations<ICommand>().ToArray())
    .WithParsed<ICommand>(c => ExecuteCommand(c, services));

static IEnumerable<Type> GetImplementations<T>()
{
    return typeof(Program).Module.GetTypes().Where(t => t.IsAssignableTo(typeof(T)));
}

static void ExecuteCommand(ICommand command, ServiceCollection services)
{
    command.Configure(services);

    ServiceProvider serviceProvider = services.BuildServiceProvider();
    command.Execute(serviceProvider);
}
