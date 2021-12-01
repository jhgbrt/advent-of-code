namespace AdventOfCode.Client;

using AdventOfCode.Common;

class Report
{
    AoCClient client;
    public Report(AoCClient client)
    {
        this.client = client;
    }
    public record Options(int? year, int? day, bool unsolved);

    public async Task Run(Options options)
    {
        (var year, var day, var unsolved) = options;

        foreach ((var y, var d) in AoCLogic.Puzzles())
        {
            if (year.HasValue && year != y) continue;
            if (day.HasValue && day != d) continue;

            var puzzle = await client.GetPuzzleAsync(y, d);
            if (unsolved && puzzle.Status == Status.Completed) continue;

            Console.WriteLine((y, d, puzzle.Status, puzzle.Answer.part1, puzzle.Answer.part2));
        }
    }

}