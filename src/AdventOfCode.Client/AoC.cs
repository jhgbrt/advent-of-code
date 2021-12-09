namespace AdventOfCode.Client;

using AdventOfCode.Client.Commands;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Spectre.Console.Cli;

using System.Reflection;

public static class AoC
{
    public static async Task InvokeAsync(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddUserSecrets(Assembly.GetEntryAssembly())
            .Build();


        var app = new CommandApp();
        app.Configure(config =>
        {
            config.AddCommand<Sync>("sync").WithDescription("");
        });

        var host = new MyHostBuilder(config).Build();

        await host.Run(args.Any() ? args : new[] { "run" });
    }
    static ITypeRegistrar RegisterServices()
    {
        var config = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddUserSecrets(Assembly.GetEntryAssembly())
            .Build();


        var cookieValue = config["AOC_SESSION"] ?? throw new Exception("This operation requires AOC_SESSION to be set as an environment variable.");
        var baseAddress = "https://adventofcode.com";

        var services = new ServiceCollection();

        services.AddTransient(_ => new AoCClient(new Configuration(baseAddress, cookieValue)));

        return new TypeRegistrar(services);
    }

}



public sealed class TypeRegistrar : ITypeRegistrar
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

public sealed class TypeResolver : ITypeResolver
{
    private readonly IServiceProvider _provider;

    public TypeResolver(IServiceProvider provider) => _provider = provider ?? throw new ArgumentNullException(nameof(provider));

    public object Resolve(Type type) => _provider.GetRequiredService(type);
}


