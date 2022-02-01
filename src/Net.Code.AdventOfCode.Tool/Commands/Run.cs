
using Net.Code.AdventOfCode.Tool.Core;

using Spectre.Console.Cli;

using System.ComponentModel;

namespace Net.Code.AdventOfCode.Tool.Commands;

[Description("Run the code for a specific puzzle.")]
class Run : ManyPuzzlesCommand<Run.Settings>
{
    private readonly IAoCRunner manager;
    private readonly IInputOutputService io;

    public Run(IAoCRunner manager, AoCLogic aocLogic, IInputOutputService io) : base(aocLogic)
    {
        this.manager = manager;
        this.io = io;
    }

    public class Settings : AoCSettings
    {
        [Description("The fully qualified name of the type containing the code for this puzzle. " +
        "Use a format string with {0} and {1} as placeholders for year and day. " +
        "(example: MyAdventOfCode.Year{0}.Day{1:00}.AoC{0}{1:00})")]
        [CommandOption("-t|--typename")]
        public string? typeName { get; set; }
    }

    public override async Task<int> ExecuteAsync(int year, int day, Settings options)
    {
        var typeName = options.typeName;
        io.WriteLine($"{year}, day {day}");
        DayResult result = await manager.Run(typeName, year, day, (part, result) => io.MarkupLine($"part {part}: {result.Value} ({result.Elapsed})"));
        return 0;
    }
}


