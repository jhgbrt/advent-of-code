namespace AdventOfCode.Client;

using System.CommandLine;
using System.CommandLine.Invocation;

static class CommandHelper
{
    static IEnumerable<Option> GetOptionsFor<T>() => from p in typeof(T).GetProperties()
                                                     select new Option(new[] { $"--{p.Name}", $"-{p.Name[0]}" }, p.Name, p.PropertyType);

    public static Command CreateCommand<TOptions>(string name, Func<TOptions, Task> handler)
    {
        var command = new Command(name);
        foreach (var option in GetOptionsFor<TOptions>())
        {
            option.IsRequired = option.ValueType.IsValueType && !option.ValueType.IsNullableType();
            command.AddOption(option);
        }
        command.Handler = CreateHandler(handler);
        return command;
    }

    static ICommandHandler CreateHandler<T>(Func<T, Task> action) => CommandHandler.Create<T>(options => action(options));
    static bool IsNullableType(this Type type) => type.IsGenericType && !type.IsGenericTypeDefinition && typeof(Nullable<>) == type.GetGenericTypeDefinition();

}






