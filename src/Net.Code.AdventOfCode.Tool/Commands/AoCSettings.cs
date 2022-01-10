
using Net.Code.AdventOfCode.Tool.Core;

using Spectre.Console;
using Spectre.Console.Cli;

using System.ComponentModel;

namespace Net.Code.AdventOfCode.Tool.Commands;

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
            if (day >= 1 && day <= 25 && DateTime.Now.Month == 12)
                return ValidationResult.Error("Puzzle not unlocked (yet?)");
            else
                return ValidationResult.Error("There is no Advent of Code puzzle on this day");
        }
        else
        {
            return base.Validate();
        }
    }

}
