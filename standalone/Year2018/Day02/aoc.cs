var input = File.ReadAllLines("input.txt");
var sw = Stopwatch.StartNew();
var part1 = Part1(input);
var part2 = Part2(input);
Console.WriteLine((part1, part2, sw.Elapsed));
(int twos, int threes) Process(int twos, int threes, string s)
{
    var g =
        from c in s
        group c by c;
    if (g.Any(x => x.Count() == 2))
        twos++;
    if (g.Any(x => x.Count() == 3))
        threes++;
    return (twos, threes);
}

string Common(string left, string right) => new string((
    from x in left.Zip(right, (l, r) => (l, r))
    where x.l == x.r
    select x.l).ToArray());
int DiffCount(string left, string right) => left.Zip(right, (l, r) => (l, r)).Aggregate(0, (int count, (char l, char r) x) => count += x.l == x.r ? 0 : 1);