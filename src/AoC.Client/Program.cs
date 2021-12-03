using AdventOfCode.Client;

using Microsoft.Extensions.Configuration;

using System.CommandLine;

var root = new RootCommand
{
    CommandHelper.CreateCommand<RunPuzzle.Options>("run", RunPuzzle),
    CommandHelper.CreateCommand<InitPuzzle.Options>("init", InitPuzzle),
    CommandHelper.CreateCommand<SyncPuzzle.Options>("sync", SyncPuzzle),
    CommandHelper.CreateCommand<PostSolution.Options>("post", PostSolution),
    CommandHelper.CreateCommand<PublishPuzzle.Options>("publish", PublishPuzzle),
    CommandHelper.CreateCommand<Report.Options>("report", Report),
};

await root.InvokeAsync(args);

static async Task RunPuzzle(RunPuzzle.Options options) => await new RunPuzzle().Run(options);
static async Task InitPuzzle(InitPuzzle.Options options) => await new InitPuzzle(Factory.CreateClient()).Run(options);
static async Task SyncPuzzle(SyncPuzzle.Options options) => await new SyncPuzzle(Factory.CreateClient()).Run(options);
static async Task PostSolution(PostSolution.Options options) => await new PostSolution(Factory.CreateClient()).Run(options);
static async Task PublishPuzzle(PublishPuzzle.Options options) => await new PublishPuzzle().Run(options);
static async Task Report(Report.Options options) => await new Report(Factory.CreateClient()).Run(options);

static class Factory
{
    public static AoCClient CreateClient()
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();
        var cookieValue = config["AOC_SESSION"];
        var baseAddress = new Uri("https://adventofcode.com");
        return new AoCClient(baseAddress, cookieValue);
    }

}
