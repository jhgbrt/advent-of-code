﻿
using Net.Code.AdventOfCode.Tool.Core;

using System.ComponentModel;

namespace Net.Code.AdventOfCode.Tool.Commands;


[Description("Sync the data (specifically the posted answers) for a puzzle. Requires AOC_SESSION set as an environment variable.")]
class Sync : ManyPuzzlesCommand<AoCSettings>
{
    private readonly IPuzzleManager manager;
    private readonly IInputOutputService io;

    public Sync(IPuzzleManager manager, AoCLogic aocLogic, IInputOutputService io) : base(aocLogic)
    {
        this.manager = manager;
        this.io = io;
    }

    public override async Task<int> ExecuteAsync(int year, int day, AoCSettings _)
    {
        io.WriteLine($"Synchronizing for puzzle {year}-{day:00}...");
        await manager.Sync(year, day);
        return 0;
    }

}