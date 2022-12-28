var items = (
    from line in File.ReadAllLines("input.txt") let indexes = line.Split(": ").Select(int.Parse).ToArray() select (layer: indexes[0], range: indexes[1])).ToArray();
var sw = Stopwatch.StartNew();
var part1 = Firewall.Severity(items);
var part2 = Firewall.DelayToEscape(items);
Console.WriteLine((part1, part2, sw.Elapsed));