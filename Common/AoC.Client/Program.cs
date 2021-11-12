
using Microsoft.Extensions.Configuration;

using NodaTime;

using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text;
using System.Text.Json;

interface IYearDay
{
    int? year { get; }
    int? day { get; } 
}
record UpdateCacheOptions(bool? force, int? leaderboardId);
record PuzzleCommandOptions(int? year, int? day, bool force);
record LeaderBoardOptions(int year, int id);
class Program
{
    static readonly IClock Clock = SystemClock.Instance;

    static IEnumerable<Option> GetOptionsFor<T>() => from p in typeof(T).GetProperties()
                                                     select new Option(new[] { $"--{p.Name}" }, p.Name, p.PropertyType);

    public static async Task<int> Main(string[] args)
    {
        Command cache = new Command("cache")
        {
            CreateCommand<UpdateCacheOptions>("update", UpdateCache)
        };


        Command puzzle = new Command("puzzle")
        {
            CreateCommand<PuzzleCommandOptions>("get", GetPuzzle),
        };

        Command leaderboard = new Command("leaderboard")
        {
            CreateCommand<LeaderBoardOptions>("get", GetLeaderBoard),
            CreateCommand<LeaderBoardOptions>("report", ReportLeaderBoard),
        };

        var root = new RootCommand
        {
            cache, puzzle, leaderboard
        };

        await root.InvokeAsync(args);
        return 0;
    }

    private static async Task GetPuzzle(PuzzleCommandOptions options)
    {
        var client = CreateClient();
        foreach ((var y, var d) in GetDays(options.year, options.day))
        {
            Console.WriteLine($"{y}/{d:00}");
            var result = await client.GetPuzzleAsync(y, d, !options.force);

            if (result == null)
                return;

            var target = Solution(y, d);
            if (!target.Exists) target.Create();

            await File.WriteAllTextAsync(Path.Combine(target.FullName, "input.txt"), result.Input, Encoding.ASCII);
            await File.WriteAllTextAsync(Path.Combine(target.FullName, "answers.json"), JsonSerializer.Serialize(result.Answer));
        }
    }    

    private static async Task UpdateCache(UpdateCacheOptions ydf)
    {
        var updatepuzzlecache = UpdatePuzzleCache(ydf.force ?? false);
        var updateleaderboardcache = ydf.leaderboardId.HasValue ? UpdateLeaderBoardCache(ydf.force ?? false, ydf.leaderboardId.Value) : Task.CompletedTask;

        await Task.WhenAll(
            updatepuzzlecache,
            updateleaderboardcache
            );
    }

    private static async Task UpdatePuzzleCache(bool force)
    {
        var puzzleRepo = CreatePuzzleRepository();
        var client = CreateClient();
        foreach ((var y, var d ) in GetDays(null, null))
        {
            var puzzle = await puzzleRepo.GetAsync(y, d);
            if (puzzle == null || force || !puzzle.Final || string.IsNullOrEmpty(puzzle.Input))
            {
                puzzle = await client.GetPuzzleAsync(y, d, false);
                await puzzleRepo.PutAsync(puzzle);
                Console.WriteLine($"update puzzle cache {y} {d} {puzzle.Status}");
            }
        }
    }

    private static async Task UpdateLeaderBoardCache(bool force, int id)
    {
        var leaderboardRepo = CreateLeaderBoardRepository();
        var client = CreateClient();

        for (var year = MinYear; year <= MaxYear; year++)
        {
            var lb = await leaderboardRepo.GetAsync(year, id);
            if (lb == null || force)
            {
                lb = await client.GetLeaderBoardAsync(year, id);
                if (lb != null)
                    await leaderboardRepo.PutAsync(lb);
                Console.WriteLine($"update lbcache {year} {id}");
            }
        }

    }

    private static async Task GetLeaderBoard(LeaderBoardOptions o)
    {
        var repo = CreateLeaderBoardRepository();
        var result = await repo.GetAsync(o.year, o.id);
        Console.WriteLine(result);
    }
    private static async Task ReportLeaderBoard(LeaderBoardOptions o)
    {
        var repo = CreateLeaderBoardRepository();
        var result = await repo.GetAsync(o.year, o.id);

        Console.WriteLine("name;stars");

        var query = from item in result.Members
                    select (item.Name, item.Stars);

        foreach (var item in query)
        {
            Console.WriteLine($"{item.Name};{item.Stars}");
        }
    }

    static Command CreateCommand<TOptions>(string name, Func<TOptions, Task> handler) 
    {
        var command = new Command(name);
        foreach (var option in GetOptionsFor<TOptions>())
        {
            option.IsRequired = false;
            command.AddOption(option);
        }
        command.Handler = CreateHandler(handler);
        return command;
    }

    static ICommandHandler CreateHandler<T>(Func<T, Task> action) 
    {
        return CommandHandler.Create<T>(async options =>
        {
                await action(options);
        });
    }

    static IEnumerable<(int year, int day)> GetDays(int? year, int? day)
    {
        if (!year.HasValue)
        {
            for (int y = MinYear; y <= MaxYear; y++)
                for (int d = 1; d <= MaxDay(y, Clock); d++)
                    yield return (y,d);
        }
        else if (!day.HasValue)
        {
            var y = year.Value;
            for (int d = 1; d <= MaxDay(y, Clock); d++)
                yield return (y, d);
        }
        else
        {
            (var y, var d) = (year.Value, day.Value);
            yield return (y, d);
        }
    }

    internal static int MinYear => 2015;
    internal static int MaxYear => Clock.GetCurrentInstant().InUtc().Year;

    internal static int MaxDay(int year, IClock clock)
    {
        var now = clock.GetCurrentInstant().InZone(DateTimeZoneProviders.Tzdb["EST"]);

        // for past years, all 25 puzzles are available
        if (year < now.Year) return 25;

        // for future years, no puzzles are available
        if (year > now.Year) return 0;

        return now.Month switch
        {
            // before december, nothing is available
            >= 1 and < 12 => 0,
            // between 1 - 25 december, puzzles are unlocked at midnight EST, so the nof available puzzles = day of month with max of 25
            12 => Math.Min(now.Day, 25),
            // invalid month, impossible
            _ => throw new Exception("invalid month")
        };
    }

    static AoCClient CreateClient()
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();
        var cookieValue = config["AOC_SESSION"];
        var baseAddress = new Uri("https://adventofcode.com");
        return new AoCClient(baseAddress, cookieValue, Cache);
    }

    static DirectoryInfo Solution(int year, int day)=> new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), year.ToString(), $"Day{day:00}"));
    static DirectoryInfo Db => new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "Common", "content", "db"));
    static DirectoryInfo Cache => new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "Common", "content", "cache"));
    static IPuzzleRepository CreatePuzzleRepository() => new PuzzleRepository(Db);
    static ILeaderboardRepository CreateLeaderBoardRepository() => new LeaderboardRepository(Db);
}



