var input = File.ReadAllLines("input.txt");
var sw = Stopwatch.StartNew();
var part1 = input.Select(s => new IPAddress(s)).Where(ip => ip.SupportsTLS()).Count();
var part2 = input.Select(s => new IPAddress(s)).Where(ip => ip.SupportsSSL()).Count();
Console.WriteLine((part1, part2, sw.Elapsed));
class AoCRegex
{
}

enum WhereAmI
{
    Outside,
    Inside
}