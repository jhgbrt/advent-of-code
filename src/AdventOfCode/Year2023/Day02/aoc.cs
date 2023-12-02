namespace AdventOfCode.Year2023.Day02;
public class AoC202302
{
    static string[] input = Read.InputLines();

    static ImmutableArray<Game> games = input.Select(s => Game.Parse(s)).ToImmutableArray();

    public int Part1() => (from game in games
                              where game.IsPossible(12, 13, 14)
                              select game).Sum(g => g.Id);
    public int Part2() => (from game in games
                              let fewest = game.Fewest()
                              select fewest.red * fewest.green * fewest.blue).Sum();
}

readonly record struct Game(int Id, Grab[] Grabs)
{
    public bool IsPossible(int red, int green, int blue) => Grabs.All(gr => gr.IsPossible(red, green, blue));
    public (int red, int green, int blue) Fewest() => (Grabs.Max(g => g.Red), Grabs.Max(g => g.Green), Grabs.Max(g => g.Blue));

    public static Game Parse(string s)
    {
        var m = Regexes.GameRegex().Match(s);

        return new Game(
            int.Parse(m.Groups["id"].Value),
            m.Groups["grabs"].Value.Split("; ").Select(Grab.Parse).ToArray()
        );
    }

    public override string ToString() => $"Game {Id}: {string.Join("; ", Grabs)}";
}
readonly record struct Grab(int Red, int Green, int Blue)
{
    public bool IsPossible(int red, int green, int blue) => red >= Red && green >= Green && blue >= Blue;
    public static Grab Parse(string s)
    {
        var split = s.Split(", ");
        int r = 0, g = 0, b = 0;
        foreach (var item in split)
        {
            var component = Regexes.ComponentRegex().Match(item);
            var n = int.Parse(component.Groups["n"].Value);
            (r, g, b) = component.Groups["color"].Value switch
            {
                "red" => (n, g, b),
                "green" => (r, n, b),
                "blue" => (r, g, n)
            };
        }
        return new(r, g, b);
    }
    public override string ToString() => $"{Red} red, {Green} green, {Blue} blue";
}

static partial class Regexes
{
    [GeneratedRegex(@"^Game (?<id>\d+): (?<grabs>.+)$")]
    public static partial Regex GameRegex();
    [GeneratedRegex(@"^(?<n>\d+) (?<color>.+)$")]
    public static partial Regex ComponentRegex();
}

public class Tests
{
    [Theory]
    [InlineData("Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green", true)]
    [InlineData("Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue", true)]
    [InlineData("Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red", false)]
    [InlineData("Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red", false)]
    [InlineData("Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green", true)]
    public void Test(string input, bool expected)
    {
        var game = Game.Parse(input);
        var result = game.IsPossible(12, 13, 14);
        Assert.Equal(expected, result);
    }

}