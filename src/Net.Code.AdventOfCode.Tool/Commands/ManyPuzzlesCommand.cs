
using Net.Code.AdventOfCode.Tool.Core;

using Spectre.Console.Cli;

namespace Net.Code.AdventOfCode.Tool.Commands;

abstract class ManyPuzzlesCommand<TSettings> : AsyncCommand<TSettings> where TSettings : CommandSettings, IAoCSettings
{
    private readonly AoCLogic AoCLogic;

    protected ManyPuzzlesCommand(AoCLogic aoCLogic)
    {
        AoCLogic = aoCLogic;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, TSettings options)
    {
        (var year, var day) = (options.year, options.day);
        foreach (var (y, d) in AoCLogic.Puzzles(year, day))
        {
            await ExecuteAsync(y, d, options);
        }
        return 0;
    }

    public abstract Task ExecuteAsync(int year, int day, TSettings options);

}
abstract class SinglePuzzleCommand<TSettings> : AsyncCommand<TSettings> where TSettings : CommandSettings, IAoCSettings
{
    public override async Task<int> ExecuteAsync(CommandContext context, TSettings options)
    {
        (var year, var day) = (options.year, options.day);
        if (!year.HasValue || !day.HasValue)
            throw new Exception("Please specify year & day explicitly");

        await ExecuteAsync(year.Value, day.Value, options);

        return 0;
    }

    public abstract Task ExecuteAsync(int year, int day, TSettings options);

}
