var input = 377;
var sw = Stopwatch.StartNew();
var part1 = Part1();
var part2 = Spinlock.FindFast(input, 50_000_000);
Console.WriteLine((part1, part2, sw.Elapsed));
object Part1()
{
    var result = Spinlock.Find(input, 2017);
    return result.buffer[result.index + 1];
}