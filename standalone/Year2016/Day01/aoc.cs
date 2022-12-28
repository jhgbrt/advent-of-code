var input = File.ReadAllText("input.txt");
var sw = Stopwatch.StartNew();
var part1 = Navigate(input).Part1;
var part2 = Navigate(input).Part2;
Console.WriteLine((part1, part2, sw.Elapsed));
Navigator Navigate(string input)
{
    var navigator = new Navigator();
    var instructions = input.Parse();
    foreach (var (direction, distance) in instructions)
    {
        navigator.Head(direction, distance);
    }

    return navigator;
}

partial class AoCRegex
{
}

public enum Bearing
{
    N = 0,
    E,
    S,
    W
}

public enum Direction
{
    L,
    R
}