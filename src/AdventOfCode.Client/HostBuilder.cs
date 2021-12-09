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
                CreateCommand<Exec, Exec.Options>(Exec),
                CreateCommand<Verify, Verify.Options>(Verify),
                CreateCommand<Init, Init.Options>(Init),
                CreateCommand<Sync, Sync.Options>(Sync),
                CreateCommand<Show, Show.Options>(Show),
                CreateCommand<Post, Post.Options>(Post),
                CreateCommand<Export, Export.Options>(Export),
                CreateCommand<Report, Report.Options>(Report),
                CreateCommand<Leaderboard, Leaderboard.Options>(Leaderboard),
            };
        }
        public Task Run(string[] args) => root.InvokeAsync(args);

        async Task Exec(Exec.Options options) => await new Exec(Factory.CreateManager(config)).Run(options);
        async Task Verify(Verify.Options options) => await new Verify(Factory.CreateClient(config), Factory.CreateManager(config)).Run(options);
        async Task Init(Init.Options options) => await new Init(Factory.CreateClient(config)).Run(options);
        async Task Sync(Sync.Options options) => await new Sync(Factory.CreateManager(config)).Run(options);
        async Task Show(Show.Options options) => await new Show(Factory.CreateClient(config)).Run(options);
        async Task Post(Post.Options options) => await new Post(Factory.CreateManager(config)).Run(options);
        async Task Export(Export.Options options) => await new Export().Run(options);
        async Task Report(Report.Options options) => await new Report(Factory.CreateClient(config)).Run(options);
        async Task Leaderboard(Leaderboard.Options options) => await new Leaderboard(Factory.CreateClient(config)).Run(options);
    }

    static Command CreateCommand<T, TOptions>(Func<TOptions, Task> handler)
    {
        var name = typeof(T).Name.ToLowerInvariant();
        var description = GetDescription(typeof(T));
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
