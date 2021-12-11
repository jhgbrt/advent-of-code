using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace AdventOfCode.Client.Commands;

public class AoCSettings : CommandSettings
{
    [Description("Year (default: current year)")]
    [CommandArgument(0, "[YEAR]")]
    public int year { get; set; } = DateTime.Now.Year;
    [Description("Day (default: current day)")]
    [CommandArgument(1, "[DAY]")]
    public int day { get; set; } = DateTime.Now.Day;

    public override ValidationResult Validate()
    {
        if (!AoCLogic.IsValidAndUnlocked(year, day))
        {
            return ValidationResult.Error("Puzzle not unlocked or invalid year/day combination");
        }
        else
        {
            return base.Validate();
        }
    }

}

[Description("Sync the data (specifically the posted answers) for a puzzle. Requires AOC_SESSION set as an environment variable.")]
class Sync : AsyncCommand<Sync.Settings>
{
    private readonly AoCManager manager;

    public Sync(AoCManager manager)
    {
        this.manager = manager;
    }
    public class Settings : AoCSettings { }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings options)
    {
        (var year, var day) = (options.year, options.day);

        if (!AoCLogic.IsValidAndUnlocked(year, day))
        {
            Console.WriteLine("Puzzle not yet unlocked");
            return 1;
        }

        var dir = FileSystem.GetDirectory(year, day);
        if (!dir.Exists)
        {
            Console.WriteLine("Puzzle not yet initialized. Use 'init' first.");
            return 1;
        }

        Console.WriteLine("Updating puzzle answers");
        await manager.Sync(year, day);
        return 0;
    }
   
}
