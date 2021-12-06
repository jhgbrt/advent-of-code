namespace AdventOfCode.Client.Commands;

using Microsoft.Extensions.Configuration;

using System.CommandLine;
using System.CommandLine.Invocation;
using System.ComponentModel;
using System.Reflection;

class HostBuilder
{
    private readonly IConfigurationRoot config;

    public HostBuilder(IConfigurationRoot config)
    {
        this.config = config;
    }

    public Host Build() => new Host(config);

    internal class Host 
    {
        private readonly RootCommand root;
        private readonly IConfigurationRoot config;
        public Host(IConfigurationRoot config)
        {
            this.config = config;
            root = new RootCommand
            {
                CreateCommand<RunPuzzle.Options>("run", GetDescription(typeof(RunPuzzle))!, RunPuzzle),
                CreateCommand<InitPuzzle.Options>("init", GetDescription(typeof(InitPuzzle))!, InitPuzzle),
                CreateCommand<SyncPuzzle.Options>("sync", GetDescription(typeof(SyncPuzzle))!, SyncPuzzle),
                CreateCommand<PostSolution.Options>("post", GetDescription(typeof(PostSolution))!, PostSolution),
                CreateCommand<ExportPuzzle.Options>("export", GetDescription(typeof(ExportPuzzle))!, ExportPuzzle),
                CreateCommand<Report.Options>("report", GetDescription(typeof(Report))!, Report),
                CreateCommand<ShowLeaderboard.Options>("leaderboard", GetDescription(typeof(ShowLeaderboard))!, ShowLeaderboard),
            };
        }
        public Task Run(string[] args) => root.InvokeAsync(args);

        async Task RunPuzzle(RunPuzzle.Options options) => await new RunPuzzle().Run(options);
        async Task InitPuzzle(InitPuzzle.Options options) => await new InitPuzzle(Factory.CreateClient(config)).Run(options);
        async Task SyncPuzzle(SyncPuzzle.Options options) => await new SyncPuzzle(Factory.CreateClient(config)).Run(options);
        async Task PostSolution(PostSolution.Options options) => await new PostSolution(Factory.CreateClient(config)).Run(options);
        async Task ExportPuzzle(ExportPuzzle.Options options) => await new ExportPuzzle().Run(options);
        async Task Report(Report.Options options) => await new Report(Factory.CreateClient(config)).Run(options);
        async Task ShowLeaderboard(ShowLeaderboard.Options options) => await new ShowLeaderboard(Factory.CreateClient(config)).Run(options);
    }


    static Command CreateCommand<TOptions>(string name, string description, Func<TOptions, Task> handler)
    {
        var command = new Command(name, description);
        foreach (var option in GetOptionsFor<TOptions>())
        {
            option.IsRequired = option.ValueType.IsValueType && !IsNullableType(option.ValueType);
            command.AddOption(option);
        }
        command.Handler = CreateHandler(handler);
        return command;
    }
    static IEnumerable<Option> GetOptionsFor<T>() => from p in typeof(T).GetProperties()
                                                     select new Option(new[] { $"--{p.Name}", $"-{p.Name[0]}" }, GetDescription(p) ?? p.Name, p.PropertyType);

    static string? GetDescription(ICustomAttributeProvider provider) => provider.GetCustomAttributes(typeof(DescriptionAttribute), false).OfType<DescriptionAttribute>().SingleOrDefault()?.Description;

    static ICommandHandler CreateHandler<T>(Func<T, Task> action) => CommandHandler.Create<T>(options => action(options));
    static bool IsNullableType(Type type) => type.IsGenericType && !type.IsGenericTypeDefinition && typeof(Nullable<>) == type.GetGenericTypeDefinition();

}
