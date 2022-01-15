using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Net.Code.AdventOfCode.Tool.UnitTests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Net.Code.AdventOfCode.Tool;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Net.Code.AdventOfCode.Tool.Commands;
using Net.Code.AdventOfCode.Tool.Core;
using Net.Code.AdventOfCode.Tool.Logic;

using NodaTime;

using Spectre.Console;
using Spectre.Console.Cli;

using System.ComponentModel;
using System.Reflection;

public static class AoC
{
    public static Task<int> RunAsync(string[] args) 
        => RunAsync(AssemblyResolver.Instance, new InputOutputService(), args);
    public static async Task<int> RunAsync(
        IAssemblyResolver resolver,
        IInputOutputService outputService,
        string[] args
        )
    {
        var config = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddUserSecrets(resolver.GetEntryAssembly())
            .Build();

        string? loglevel = null;
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].StartsWith("--loglevel="))
            {
                loglevel = args[i].Split('=')[1];
                break;
            }
            else if (args[i] == "--loglevel" && i < args.Length - 1)
            {
                loglevel = args[i + 1];
                break;
            }
        }

        var cookieValue = config["AOC_SESSION"] ?? throw new Exception("This operation requires AOC_SESSION to be set as an environment variable or user secret.");
        var baseAddress = "https://adventofcode.com";
        var configuration = new Configuration(baseAddress, cookieValue);
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddInlineSpectreConsole(c => c.LogLevel = LogLevel.Trace).SetMinimumLevel(string.IsNullOrEmpty(loglevel) ? LogLevel.Warning : Enum.Parse<LogLevel>(loglevel, true)));
        services.AddSingleton(configuration);
        services.AddTransient<IAoCClient, AoCClient>();
        services.AddTransient<IPuzzleManager, PuzzleManager>();
        services.AddTransient<IAoCRunner, AoCRunner>();
        services.AddTransient<ICodeManager, CodeManager>();
        services.AddTransient<IReportManager, ReportManager>();
        services.AddTransient<ICache, Cache>();
        services.AddTransient<IFileSystem, FileSystem>();
        services.AddTransient<IAssemblyResolver, AssemblyResolver>();
        services.AddTransient<IHttpClientWrapper, HttpClientWrapper>();
        services.AddTransient<AoCLogic>();
        services.AddSingleton(SystemClock.Instance);
        services.AddSingleton(outputService);
        foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(t => !t.IsAbstract && t.IsAssignableTo(typeof(ICommand))))
        {
            services.AddTransient(type);
        }
        foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(t => !t.IsAbstract && t.IsAssignableTo(typeof(CommandSettings))))
        {
            services.AddTransient(type);
        }

        var registrar = new TypeRegistrar(services);

        var app = new CommandApp(registrar);

        app.Configure(config =>
        {
            AddCommand<Run>(config);
            AddCommand<Verify>(config);
            AddCommand<Init>(config);
            AddCommand<Sync>(config);
            AddCommand<Show>(config);
            AddCommand<Post>(config);
            AddCommand<Export>(config);
            AddCommand<Report>(config);
            AddCommand<Leaderboard>(config);
            AddCommand<Stats>(config);
            if (args.Contains("--debug"))
                config.PropagateExceptions();
        });

        return await app.RunAsync(args);
    }

    static ICommandConfigurator AddCommand<T>(IConfigurator config) where T : class, ICommand
        => config.AddCommand<T>(typeof(T).Name.ToLower()).WithDescription(GetDescription(typeof(T)) ?? typeof(T).Name);

    static string? GetDescription(ICustomAttributeProvider provider)
        => provider.GetCustomAttributes(typeof(DescriptionAttribute), false).OfType<DescriptionAttribute>().SingleOrDefault()?.Description;


}

sealed class TypeRegistrar : ITypeRegistrar
{
    private readonly IServiceCollection _builder;

    public TypeRegistrar(IServiceCollection builder)
    {
        _builder = builder;
    }

    public ITypeResolver Build() => new TypeResolver(_builder.BuildServiceProvider());

    public void Register(Type service, Type implementation) => _builder.AddTransient(service, implementation);

    public void RegisterInstance(Type service, object implementation) => _builder.AddTransient(service, _ => implementation);

    public void RegisterLazy(Type service, Func<object> factory) => _builder.AddTransient(service, _ => factory());
}

sealed class TypeResolver : ITypeResolver
{
    private readonly IServiceProvider _provider;

    public TypeResolver(IServiceProvider provider) 
        => _provider = provider ?? throw new ArgumentNullException(nameof(provider));

    public object Resolve(Type? type) 
        => _provider.GetRequiredService(type ?? throw new ArgumentNullException(nameof(type)));
}


