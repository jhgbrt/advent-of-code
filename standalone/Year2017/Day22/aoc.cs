var input = File.ReadAllLines("input.txt").ToArray();
var sw = Stopwatch.StartNew();
var part1 = Part1();
var part2 = Part2();
Console.WriteLine((part1, part2, sw.Elapsed));
object Part1()
{
    var grid = input.ToRectangular();
    return new Grid(grid).InfectGrid(10000);
}

object Part2()
{
    var grid = input.ToRectangular();
    return new Grid(grid).InfectGrid2(10000000);
}