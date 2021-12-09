using Spectre.Console.Cli;
using System.ComponentModel;

namespace AdventOfCode.Client.Commands;

[Description("Sync the data (specifically the posted answers) for a puzzle. Requires AOC_SESSION set as an environment variable.")]
class Sync : AsyncCommand<Sync.Settings>
{
    private readonly AoCManager manager;

    public Sync(AoCManager manager)
    {
        this.manager = manager;
    }

    public class Settings : CommandSettings
    {
        [Description("Year (default: current year)")]
        [CommandOption("-y|--year")]
        public int? year { get; set; }
        [Description("Day (default: current day)")]
        [CommandOption("-d|--day")]
        public int? day { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings options)
    {
        (var year, var day) = (options.year ?? DateTime.Now.Year, options.day ?? DateTime.Now.Day);

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
