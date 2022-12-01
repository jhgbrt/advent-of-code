﻿
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
        int result = 0;
        foreach (var (y, d) in AoCLogic.Puzzles(year, day))
        {
            var v = await ExecuteAsync(y, d, options);
            if (v != 0)
                result = v;
        }
        return result;
    }

    public abstract Task<int> ExecuteAsync(int year, int day, TSettings options);

}
abstract class SinglePuzzleCommand<TSettings> : AsyncCommand<TSettings> where TSettings : CommandSettings, IAoCSettings
{
    private readonly AoCLogic AoCLogic;

    protected SinglePuzzleCommand(AoCLogic aoCLogic)
    {
        AoCLogic = aoCLogic;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, TSettings options)
    {
        (var year, var day) = (options.year, options.day);
        if (!year.HasValue || !day.HasValue)
        {
            year = AoCLogic.Year;
            day = AoCLogic.Day;
        }

        if (!year.HasValue || !day.HasValue)
            throw new Exception("Please specify year & day explicitly");

        if (!AoCLogic.IsValidAndUnlocked(year.Value, day.Value))
            throw new Exception($"Not a valid puzzle: {year}/{day}");

        return await ExecuteAsync(year.Value, day.Value, options);
    }

    public abstract Task<int> ExecuteAsync(int year, int day, TSettings options);

}