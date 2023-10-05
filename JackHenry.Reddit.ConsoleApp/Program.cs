using CommandLine;
using JackHenry.Reddit.ConsoleApp;

Parser.Default.ParseArguments(args, GetImplementations<ICommand>().ToArray())
    .WithParsed<ICommand>(a => a.Execute());

static IEnumerable<Type> GetImplementations<T>()
{
    return typeof(Program).Module.GetTypes().Where(t => t.IsAssignableTo(typeof(T)));
}
