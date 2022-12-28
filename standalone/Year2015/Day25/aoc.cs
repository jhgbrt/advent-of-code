var input = File.ReadAllLines("input.txt");
var row = 3010;
var column = 3019;
var code = 20151125;
var m = 252533;
var d = 33554393;
var sw = Stopwatch.StartNew();
var part1 = Part1();
var part2 = "";
Console.WriteLine((part1, part2, sw.Elapsed));
object Part1()
{
    var value = code;
    (var r, var c) = (1, 1);
    while (true)
    {
        (r, c) = (r - 1, c + 1);
        if (r == 0)
            (r, c) = (c, 1);
        value = (m * value) % d;
        if ((r, c) == (row, column))
            return value;
    }
}

partial class AoCRegex
{
}