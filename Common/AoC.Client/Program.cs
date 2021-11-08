
using Ardalis.GuardClauses;

using Microsoft.Extensions.Configuration;

using NodaTime;

using System.CommandLine;
using System.CommandLine.Invocation;

class Program
{
    static readonly IClock Clock = SystemClock.Instance;

    public static async Task<int> Main(string[] args)
    {
        var root = new RootCommand
        {
            new Command("cache")
            {
                new Command("update") { Handler = CreateHandler(UpdateCache) }
            },
            new Command("puzzle")
            {
                new Command("show-answers") { Handler = CreateHandler(ShowAnswers) },
                new Command("show-status") { Handler = CreateHandler(ShowStatus) },
                new Command("get") { Handler = CreateHandler(GetPuzzle) }
            }
        };

        root.AddGlobalOption(new Option<int>("--year"));
        root.AddGlobalOption(new Option<int>("--day"));
        root.AddGlobalOption(new Option<bool>("--force"));
        root.AddGlobalOption(new Option<DirectoryInfo>("--dir"));

        await root.InvokeAsync(args);
        return 0;
    }

    private static async Task GetPuzzle(int y, int d, bool f, DirectoryInfo dir)
    {
        var repo = CreateRepository(dir);
        var result = await repo.GetAsync(y, d);
        Console.WriteLine(result);
    }    

    private static async Task ShowAnswers(int y, int d, bool f, DirectoryInfo dir)
    {
        var repo = CreateRepository(dir);
        var result = await repo.GetAsync(y, d);
        if (result != null)
        {
            Console.WriteLine($"{y};{d:00};{result.Answer.part1};{result.Answer.part2}");
        }
    }
    private static async Task ShowStatus(int y, int d, bool f, DirectoryInfo dir)
    {
        var repo = CreateRepository(dir);
        var result = await repo.GetAsync(y, d);
        Console.WriteLine($"{y};{d:00};{result?.Status}");
    }

    private static async Task UpdateCache(int y, int d, bool f, DirectoryInfo dir)
    {
        var repo = CreateRepository(dir);
        var client = CreateClient(dir);

        Console.Write($"update: {y} {d} - ");

        var result = await repo.GetAsync(y, d);

        if (f)
        {
            Console.Write("forced update - ");
            result = await client.GetAsync(y, d, false);
            await repo.PutAsync(result);
        }
        else if (result == null)
        {
            Console.Write("not in cache - ");
            result = await client.GetAsync(y, d, false);
            await repo.PutAsync(result);
        }
        else if (!result.Final)
        {
            Console.Write("not yet final - ");
            result = await client.GetAsync(y, d, false);
            await repo.PutAsync(result);
        }
        else
        {
            Console.Write("in cache - ");
        }
        Console.WriteLine(result.Status);
    }

    static ICommandHandler CreateHandler(Func<int,int, bool, DirectoryInfo, Task> action) => CommandHandler.Create<int?, int?, bool?, DirectoryInfo>(async (year, day, force, dir) =>
    {
        Guard.Against.InvalidInput(dir, nameof(dir), d => d.Exists, "invalid directory");
        var f = force ?? false;

        if (!year.HasValue)
        {
            for (int y = 2015; y <= Clock.GetCurrentInstant().InUtc().Year; y++)
                for (int d = 1; d <= MaxDay(y, Clock); d++)
                    await action(y, d, f, dir);
        }
        else if (!day.HasValue)
        {
            var y = year.Value;
            for (int d = 1; d <= MaxDay(y, Clock); d++)
                await action(y, d, f, dir);
        }
        else
        {
            (var y, var d) = (year.Value, day.Value);
            await action(y, d, f, dir);
        }
    });

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

    static AoCClient CreateClient(DirectoryInfo baseDirectory)
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();
        var cookieValue = config["AOC_SESSION"];
        var baseAddress = new Uri("https://adventofcode.com");
        return new AoCClient(baseAddress, cookieValue, baseDirectory.GetDirectories("cache").Single());
    }

    static IPuzzleRepository CreateRepository(DirectoryInfo baseDirectory) => new PuzzleRepository(baseDirectory.GetDirectories("db").Single());
}



