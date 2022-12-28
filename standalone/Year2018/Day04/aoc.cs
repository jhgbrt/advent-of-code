var input = File.ReadAllLines("input.txt");
var sw = Stopwatch.StartNew();
var part1 = Part1(input);
var part2 = Part2(input);
Console.WriteLine((part1, part2, sw.Elapsed));
internal record struct GuardAction(int ID, DateTime TimeStamp, GuardAction.Type ActionType)
{
    public enum Type
    {
        StartShift = 'G',
        FellAsleep = 'f',
        WakesUp = 'w'
    }
}

public enum Type
{
    StartShift = 'G',
    FellAsleep = 'f',
    WakesUp = 'w'
}

public enum Type
{
    StartShift = 'G',
    FellAsleep = 'f',
    WakesUp = 'w'
}