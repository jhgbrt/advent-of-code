namespace AdventOfCode.Client;

using System.CommandLine;
using System.CommandLine.Invocation;

static class CommandHelper
{
    static IEnumerable<Option> GetOptionsFor<T>() => from p in typeof(T).GetProperties()
                                                     select new Option(new[] { $"--{p.Name}" }, p.Name, p.PropertyType);

    public static Command CreateCommand<TOptions>(string name, Func<TOptions, Task> handler)
    {
        var command = new Command(name);
        foreach (var option in GetOptionsFor<TOptions>())
        {
            option.IsRequired = false;
            command.AddOption(option);
        }
        command.Handler = CreateHandler(handler);
        return command;
    }

    static ICommandHandler CreateHandler<T>(Func<T, Task> action) => CommandHandler.Create<T>(async options =>
    {
        await action(options);
    });

}






