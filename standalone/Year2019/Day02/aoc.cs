var input = File.ReadAllLines("input.txt");
var sw = Stopwatch.StartNew();
var part1 = Part1(input);
var part2 = Part2(input);
Console.WriteLine((part1, part2, sw.Elapsed));
long[] Parse(string[] input) => input[0].Split(',').Select(long.Parse).ToArray();
long[] Run(long[] range, int p1, int p2)
{
    range[1] = p1;
    range[2] = p2;
    int index = 0;
    while (range[index] != 99)
    {
        var result = range[index] switch
        {
            1 => range[range[index + 1]] + range[range[index + 2]],
            2 => range[range[index + 1]] * range[range[index + 2]],
            _ => throw new Exception()
        };
        var position = range[index + 3];
        range[position] = result;
        index += 4;
    }

    return range;
}