var lines = File.ReadAllLines("input.txt");
var input = lines.Last();
var replacements = (
    from line in lines.TakeWhile(line => !string.IsNullOrEmpty(line)) let split = line.Split(" => ") select new Replacement(split[0], split[1])).ToImmutableList();
var sw = Stopwatch.StartNew();
var part1 = Part1(input, replacements);
var part2 = Part2(input);
Console.WriteLine((part1, part2, sw.Elapsed));
int Count(string str, string element)
{
    var count = 0;
    for (var index = str.IndexOf(element); index >= 0; index = str.IndexOf(element, index + 1), ++count)
    {
    }

    return count;
}

record Replacement(string From, string To);
partial class AoCRegex
{
}