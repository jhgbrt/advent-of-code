using Microsoft.Extensions.Configuration;

namespace AdventOfCode.Client;

static class Factory
{
    public static AoCClient CreateClient(IConfigurationRoot config)
    {
        var cookieValue = config["AOC_SESSION"] ?? throw new Exception("This operation requires AOC_SESSION to be set as an environment variable.");
        var baseAddress = "https://adventofcode.com";
        return new AoCClient(new Configuration(baseAddress, cookieValue));
    }

}




