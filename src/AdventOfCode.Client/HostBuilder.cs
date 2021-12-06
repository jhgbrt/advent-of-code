namespace AdventOfCode.Client.Commands;

using Microsoft.Extensions.Configuration;

using System.CommandLine;
using System.CommandLine.Invocation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
                CreateCommand<Exec.Options>("run", GetDescription(typeof(Exec))!, Exec),
                CreateCommand<Init.Options>("init", GetDescription(typeof(Init))!, Init),
                CreateCommand<Sync.Options>("sync", GetDescription(typeof(Sync))!, Sync),
                CreateCommand<Post.Options>("post", GetDescription(typeof(Post))!, Post),
                CreateCommand<Export.Options>("export", GetDescription(typeof(Export))!, Export),
                CreateCommand<Report.Options>("report", GetDescription(typeof(Report))!, Report),
                CreateCommand<Leaderboard.Options>("leaderboard", GetDescription(typeof(Leaderboard))!, Leaderboard),
            };
        }
        public Task Run(string[] args) => root.InvokeAsync(args);

        async Task Exec(Exec.Options options) => await new Exec().Run(options);
        async Task Init(Init.Options options) => await new Init(Factory.CreateClient(config)).Run(options);
        async Task Sync(Sync.Options options) => await new Sync(Factory.CreateClient(config)).Run(options);
        async Task Post(Post.Options options) => await new Post(Factory.CreateClient(config)).Run(options);
        async Task Export(Export.Options options) => await new Export().Run(options);
        async Task Report(Report.Options options) => await new Report(Factory.CreateClient(config)).Run(options);
        async Task Leaderboard(Leaderboard.Options options) => await new Leaderboard(Factory.CreateClient(config)).Run(options);
    }

    static Command CreateCommand<TOptions>(string name, string description, Func<TOptions, Task> handler)
    {
        var command = new Command(name, description)
        {
            Handler = CommandHandler.Create(handler)
        };
        foreach (var (option, propertyInfo) in GetOptionsFor<TOptions>())
        {
            option.IsRequired = (option.ValueType.IsValueType && !IsNullableType(option.ValueType)) || IsRequired(propertyInfo);
            command.AddOption(option);
        }
        return command;
    }
    static IEnumerable<(Option, PropertyInfo)> GetOptionsFor<T>()
        => from p in typeof(T).GetProperties()
           select (new Option(new[] { $"--{p.Name}", $"-{p.Name[0]}" }, GetDescription(p) ?? p.Name, p.PropertyType), p);

    static string? GetDescription(ICustomAttributeProvider provider) 
        => provider.GetCustomAttributes(typeof(DescriptionAttribute), false).OfType<DescriptionAttribute>().SingleOrDefault()?.Description;

    static bool IsRequired(PropertyInfo p)
        => p.GetCustomAttributes<RequiredAttribute>().Any();

    static bool IsNullableType(Type type)
        => type.IsGenericType && !type.IsGenericTypeDefinition && typeof(Nullable<>) == type.GetGenericTypeDefinition();

}
