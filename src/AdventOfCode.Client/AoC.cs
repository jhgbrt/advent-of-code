namespace AdventOfCode.Client;

using Microsoft.Extensions.Configuration;

using System.Reflection;

public static class AoC
{
    public static async Task InvokeAsync(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddUserSecrets(Assembly.GetEntryAssembly())
            .Build();

        var host = new HostBuilder(config).Build();

        await host.Run(args.Any() ? args : new[] { "run" });
    }
}




