var components = (
    from line in File.ReadAllLines("input.txt") select Component.Parse(line)).ToImmutableList();
var sw = Stopwatch.StartNew();
var part1 = Bridge.Strongest(components);
var part2 = Bridge.Longest(components).strength;
Console.WriteLine((part1, part2, sw.Elapsed));