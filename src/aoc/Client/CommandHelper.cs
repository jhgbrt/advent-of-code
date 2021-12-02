namespace AdventOfCode.Client;

using System.CommandLine;
using System.CommandLine.Invocation;

using Microsoft.Extensions.Configuration;

static class CommandHelper
{
    public static RootCommand CreateRootCommand()
    {
        var root = new RootCommand
        {
            CreateCommand<RunPuzzle.Options>("run", "run the code for a specific puzzle", RunPuzzle),
            CreateCommand<InitPuzzle.Options>("init", "initialize the code for a specific puzzle. Requires AOC_SESSION set as a user secret", InitPuzzle),
            CreateCommand<SyncPuzzle.Options>("sync", "sync the data (specifically the posted answers) for a puzzle. Requires AOC_SESSION set as a user secret", SyncPuzzle),
            CreateCommand<PostSolution.Options>("post", "post an answer for a puzzle part. Requires AOC_SESSION set as a user secret", PostSolution),
            CreateCommand<ExportPuzzle.Options>("export", "export the code for a puzzle to a stand-alone C# project", ExportPuzzle),
            CreateCommand<Report.Options>("report", "show a list of all puzzles, their status (unlocked, answered), and the answers posted", Report),
            CreateCommand<ShowLeaderboard.Options>("leaderboard", "show some stats from the configured private leaderboard. Set AOC_LEADERBOARD_ID as a user-secret.", ShowLeaderboard),
        };
        return root;
    }
    static async Task RunPuzzle(RunPuzzle.Options options) => await new RunPuzzle().Run(options);
    static async Task InitPuzzle(InitPuzzle.Options options) => await new InitPuzzle(Factory.CreateClient()).Run(options);
    static async Task SyncPuzzle(SyncPuzzle.Options options) => await new SyncPuzzle(Factory.CreateClient()).Run(options);
    static async Task PostSolution(PostSolution.Options options) => await new PostSolution(Factory.CreateClient()).Run(options);
    static async Task ExportPuzzle(ExportPuzzle.Options options) => await new ExportPuzzle().Run(options);
    static async Task Report(Report.Options options) => await new Report(Factory.CreateClient()).Run(options);
    static async Task ShowLeaderboard(ShowLeaderboard.Options options) => await new ShowLeaderboard(Factory.CreateClient(), Factory.LeaderboardID).Run(options);

    static Command CreateCommand<TOptions>(string name, string description, Func<TOptions, Task> handler)
    {
        var command = new Command(name, description);
        foreach (var option in GetOptionsFor<TOptions>())
        {
            option.IsRequired = option.ValueType.IsValueType && !option.ValueType.IsNullableType();
            command.AddOption(option);
        }
        command.Handler = CreateHandler(handler);
        return command;
    }
    static IEnumerable<Option> GetOptionsFor<T>() => from p in typeof(T).GetProperties()
                                                     select new Option(new[] { $"--{p.Name}", $"-{p.Name[0]}" }, p.Name, p.PropertyType);

    static ICommandHandler CreateHandler<T>(Func<T, Task> action) => CommandHandler.Create<T>(options => action(options));
    static bool IsNullableType(this Type type) => type.IsGenericType && !type.IsGenericTypeDefinition && typeof(Nullable<>) == type.GetGenericTypeDefinition();

}


static class Factory
{
    static IConfigurationRoot config = new ConfigurationBuilder()
        .AddUserSecrets<Program>()
        .Build();

    public static int LeaderboardID => int.Parse(config["AOC_LEADERBOARD_ID"]);

    public static AoCClient CreateClient()
    {
        var cookieValue = config["AOC_SESSION"];
        var baseAddress = new Uri("https://adventofcode.com");
        return new AoCClient(baseAddress, cookieValue);
    }

}




