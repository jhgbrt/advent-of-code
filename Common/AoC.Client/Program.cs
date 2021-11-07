
using Microsoft.Extensions.Configuration;

using System.CommandLine;
using System.CommandLine.Invocation;

class Program
{
    public static async Task<int> Main(string[] args)
    {
        Command cache;
        Command puzzle;

        cache = new Command(nameof(cache))
        {
            new Command("update")
            {
                Handler = CreateHandler(async (y, d, f, client, repo) =>
                {
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
                })
            }
        };

        puzzle = new Command(nameof(puzzle))
        {
            new Command("show-answers")
            {
                Handler = CreateHandler(async (y, d, f, client, repo) =>
                {
                    var result = await repo.GetAsync(y, d);
                    if (result != null)
                    {
                        Console.WriteLine($"{y};{d:00};{result.Answer.part1};{result.Answer.part2}");
                    }
                })
            },
            new Command("show-status")
            {
                Handler = CreateHandler(async (y, d, f, client, repo) =>
                {
                    var result = await repo.GetAsync(y, d);
                    Console.WriteLine($"{y};{d:00};{result?.Status}");
                })
            },
            new Command("get")
            {
                Handler = CreateHandler(async (y, d, f, client, repo) =>
                {
                    var result = await repo.GetAsync(y, d);
                    Console.WriteLine(result);
                })
            }
        };




        var root = new RootCommand("aoc")
        {
            cache,
            puzzle
        };

        root.AddGlobalOption(new Option<int>("--year"));
        root.AddGlobalOption(new Option<int>("--day"));
        root.AddGlobalOption(new Option<bool>("--force"));
        root.AddGlobalOption(new Option<DirectoryInfo>("--dir"));

        await root.InvokeAsync(args);


        //if (string.IsNullOrEmpty(baseDirectoryStr) || !Directory.Exists(baseDirectoryStr))
        //{
        //    Console.Error.WriteLine("provide valid base directory");
        //    return 1;
        //}

        //if (string.IsNullOrEmpty(cookieValue))
        //{
        //    Console.Error.WriteLine("session cookie not found");
        //    return 1;
        //}


        //int year = -1;
        //int day = -1;
        //if (!string.IsNullOrEmpty(yearStr))
        //{
        //    year = int.Parse(yearStr);
        //}
        //if (!string.IsNullOrEmpty(dayStr))
        //{
        //    day = int.Parse(dayStr);
        //}


        //var cacheDirectory = new DirectoryInfo(Path.Combine(baseDirectoryStr, "cache"));
        //var dbDirectory = new DirectoryInfo(Path.Combine(baseDirectoryStr, "db"));
        //using var client = new AoCClient(new Uri("https://adventofcode.com"), cookieValue, cacheDirectory);

        //IPuzzleRepository repository = new PuzzleRepository(dbDirectory);



        return 0;
    }
    static ICommandHandler CreateHandler(Func<int,int, bool, AoCClient, IPuzzleRepository, Task> action) => CommandHandler.Create<int?, int?, bool?, DirectoryInfo>(async (year, day, force, dir) =>
    {
        if (!dir.Exists)
        {
            throw new Exception("Provide a valid directory");
        }
        var config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();
        var cookieValue = config["AOC_SESSION"];
        var baseAddress = new Uri("https://adventofcode.com");
        var client = CreateClient(baseAddress, cookieValue, dir);
        var repo = CreateRepository(dir);
        await Handle(year, day, force, async (int y, int d, bool force) =>
        {
            await action(y, d, force, client, repo);
        });
    });

    static async Task Handle(int? year, int? day, bool? force, Func<int, int, bool, Task> action)
    {
        if (!year.HasValue)
        {
            for (int y = 2015; y <= DateTime.Now.Year; y++)
            {
                for (int d = 1; d <= MaxDay(y); d++)
                    await action(y, d, force ?? false);
            }
        }
        else if (!day.HasValue)
        {
            var y = year.Value;
            for (int d = 1; d <= MaxDay(y); d++)
                await action(y, d, force ?? false);
        }
        else
        {
            await action(year.Value, day.Value, force ?? false);
        }

    }
    static int MaxDay(int year) => year < DateTime.Now.Year ? 25 : DateTime.Now.Month < 12 ? 0 : Math.Max(DateTime.Now.Day, 25);

    static AoCClient CreateClient(Uri baseAddress, string sessionCookie, DirectoryInfo baseDirectory) => new AoCClient(baseAddress, sessionCookie, baseDirectory.GetDirectories("cache").Single());
    static IPuzzleRepository CreateRepository(DirectoryInfo baseDirectory) => new PuzzleRepository(baseDirectory.GetDirectories("db").Single());
}



