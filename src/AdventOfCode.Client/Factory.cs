using Microsoft.Extensions.Configuration;

using System.Reflection;

namespace AdventOfCode.Client;

static class Factory
{
    static IConfigurationRoot config = new ConfigurationBuilder()
        .AddEnvironmentVariables()
        .AddUserSecrets(Assembly.GetEntryAssembly())
        .Build();

    public static AoCClient CreateClient()
    {
        var cookieValue = config["AOC_SESSION"] ?? throw new Exception("This operation requires AOC_SESSION to be set as an environment variable.");
        var baseAddress = new Uri("https://adventofcode.com");
        return new AoCClient(baseAddress, cookieValue);
    }

}




