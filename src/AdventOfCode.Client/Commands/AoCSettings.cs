using AdventOfCode.Client.Logic;

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
