var input = File.ReadAllLines("input.txt");
var sw = Stopwatch.StartNew();
var part1 = Part1(input, 20);
var part2 = Part2(input);
Console.WriteLine((part1, part2, sw.Elapsed));
int Calculate(long generations, string initialState, (string, char)[] rules)
{
    var zero = 0;
    string result = initialState;
    for (long i = 0; i < generations; i++)
    {
        if (result[0..5].Contains('#') || result[^5..^0].Contains('#'))
        {
            result = "....." + result + ".....";
            zero += 5;
        }

        result = Transform(result, rules);
    }

    return result.Select((c, i) => (c, n: i - zero)).Where(x => x.c == '#').Select(c => c.n).Sum();
}

string Transform(string input, (string pattern, char r)[] rules)
{
    char[] result = Enumerable.Repeat('.', input.Length).ToArray();
    var q =
        from r in rules
        from i in Enumerable.Range(0, input.Length - r.pattern.Length)
        where r.pattern.SequenceEqual(input.Skip(i).Take(r.pattern.Length))
        select (i: i + 2, c: r.r);
    foreach (var x in q)
    {
        result[x.i] = x.c;
    }

    return new string(result);
}