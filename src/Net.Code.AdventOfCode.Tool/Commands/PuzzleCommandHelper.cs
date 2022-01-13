namespace Net.Code.AdventOfCode.Tool.Commands;

using Net.Code.AdventOfCode.Tool.Core;

static class PuzzleCommandHelper 
{
    public static Task<int> RunMultiPuzzle<TSettings>(TSettings settings, Func<int, int, Task> action) where TSettings : AoCSettings
        => RunMultiPuzzle(settings.year, settings.day, action);
    public static async Task<int> RunMultiPuzzle(int? year, int? day, Func<int, int, Task> action)
    {
        foreach (var (y, d) in AoCLogic.Puzzles(year, day))
        {
            await action(y, d);
        }
        return 0;
    }
    public static Task<int> RunSinglePuzzle<TSettings>(TSettings settings, Func<int, int, Task> action) where TSettings : AoCSettings
        => RunSinglePuzzle(settings.year, settings.day, action);
    public static async Task<int> RunSinglePuzzle(int? year, int? day, Func<int, int, Task> action) 
    {
        if (!year.HasValue || !day.HasValue)
            throw new Exception("Please specify year & day explicitly");

        await action(year.Value, day.Value);

        return 0;
    }
}
