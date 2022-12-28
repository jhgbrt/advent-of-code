var input = new[] { 0, 1, 4, 13, 15, 12, 16 };
var sw = Stopwatch.StartNew();
var part1 = Run(input, 2020);
var part2 = Run(input, 30000000);
Console.WriteLine((part1, part2, sw.Elapsed));
long Run(int[] input, int max)
{
    var dic = input.Select((n, i) => (n, i)).ToDictionary(x => x.n, x => (turn_1: x.i, turn_2: x.i));
    int last = input.Last();
    for (var i = input.Length; i < max; i++)
    {
        var next = dic[last].turn_1 - dic[last].turn_2;
        var previous = dic.ContainsKey(next) ? dic[next].turn_1 : i;
        dic[next] = (i, previous);
        last = next;
    }

    return last;
}