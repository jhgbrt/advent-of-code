namespace AdventOfCode.Client;

using AdventOfCode.Common;

class PostSolution
{
    private readonly AoCClient client;

    public PostSolution(AoCClient client)
    {
        this.client = client;
    }
    public record Options(int year, int day, int part, string value);
    public async Task Run(Options options)
    {
        (var year, var day, var part, var value) = options;

        if (!AoCLogic.IsValidAndUnlocked(year, day))
        {
            Console.WriteLine("Invalid year/day combination. Use --year [YEAR] --day [DAY].");
            return;
        }

        if (part is not 1 or 2)
        {
            Console.WriteLine("use --part 1 or --part 2");
            return;
        }
        if (string.IsNullOrEmpty(value))
        {
            Console.WriteLine("No value provided. Use --value [value]");
        }

        var result = await client.PostAnswerAsync(year, day, part, value);

        Console.WriteLine(result.status);
        Console.WriteLine(result.content);
    }
}


