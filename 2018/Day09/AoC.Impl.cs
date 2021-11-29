namespace AdventOfCode.Year2018.Day09;

partial class AoC
{
    static string[] input = File.ReadAllLines("input.txt");

    internal static Result Part1() => Run(() => Part1(465, 71498));
    internal static Result Part2() => Run(() => Part2(465, 71498));
    public static long Part1(int players, long marbles)
    {
        var game = new Game(players);
        game.Play(marbles);
        return game.HighScore();
    }

    public static long Part2(int players, long marbles) => Part1(players, marbles * 100);

}