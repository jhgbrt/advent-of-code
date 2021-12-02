namespace AdventOfCode.Client;

class SyncPuzzle
{
    private readonly AoCClient client;

    public SyncPuzzle(AoCClient client)
    {
        this.client = client;
    }
    public record Options(int year, int day, bool force);
    public async Task Run(Options options)
    {
        (var year, var day, var force) = options;

        if (!AoCLogic.IsValidAndUnlocked(year, day))
        {
            Console.WriteLine("Puzzle not yet unlocked");
            return;
        }

        var dir = AoCLogic.GetDirectory(year, day);
        if (!dir.Exists) 
        {
            Console.WriteLine("Puzzle not yet initialized. Use 'init' first.");
            return;
        }

        Console.WriteLine("Updating puzzle answers");
        var answers = AoCLogic.GetFileName(year, day, "answers.json");
        var puzzle = await client.GetPuzzleAsync(year, day, false);
        var answer = puzzle.Answer;
        File.WriteAllText(answers, JsonSerializer.Serialize(answer));
    }
}
