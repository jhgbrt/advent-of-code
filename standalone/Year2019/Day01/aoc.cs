var input = File.ReadAllLines("input.txt");
var sw = Stopwatch.StartNew();
var part1 = Part1(input);
var part2 = Part2(input);
Console.WriteLine((part1, part2, sw.Elapsed));
int CalculateFuel1(int mass) => mass / 3 - 2;
int CalculateFuel2(int mass) => Fuel(mass).Sum();
IEnumerable<int> Fuel(int mass)
{
    var fuel = mass;
    while (true)
    {
        fuel = CalculateFuel1(fuel);
        if (fuel < 0)
            break;
        yield return fuel;
    }
}