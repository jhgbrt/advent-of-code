namespace AdventOfCode.Client;

static class Factory
{
    public static AoCClient CreateClient()
    {
        var cookieValue = Environment.GetEnvironmentVariable("AOC_SESSION") ?? throw new Exception("This operation requires AOC_SESSION to be set as an environment variable.");
        var baseAddress = new Uri("https://adventofcode.com");
        return new AoCClient(baseAddress, cookieValue);
    }

}




