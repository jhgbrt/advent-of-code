namespace AdventOfCode.Year2016.Day01;

public class AoCImpl : AoCBase
{
    public static string input = Read.InputText(typeof(AoCImpl));

    public override object Part1() => Navigate(input).Part1;
    public override object Part2() => Navigate(input).Part2;

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


