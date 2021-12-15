namespace AdventOfCode.Year2016.Day01;

public class AoC201601
{
    public static string input = Read.InputText();

    public object Part1() => Navigate(input).Part1;
    public object Part2() => Navigate(input).Part2;

    internal static Navigator Navigate(string input)
    {
        var navigator = new Navigator();

        var instructions = input.Parse();
        foreach (var (direction, distance) in instructions)
        {
            navigator.Head(direction, distance);
        }

        return navigator;
    }

}



public enum Bearing
{
    N = 0, E, S, W
}
public enum Direction
{
    L, R
}


