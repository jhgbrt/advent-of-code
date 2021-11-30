namespace AdventOfCode.Year2018.Day09;

public class AoC201809 : AoCBase
{
    static string[] input = Read.InputLines(typeof(AoC201809));

    public override object Part1() => Part1(465, 71498);
    public override object Part2() => Part2(465, 71498);
    public static long Part1(int players, long marbles)
    {
        var game = new Game(players);
        game.Play(marbles);
        return game.HighScore();
    }

    public static long Part2(int players, long marbles) => Part1(players, marbles * 100);

}