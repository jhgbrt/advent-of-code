var input = File.ReadAllLines("input.txt");
var program = input[0].Split(',').Select(int.Parse).ToImmutableArray();
var sw = Stopwatch.StartNew();
var part1 = (
    from p in GetPermutations(Range(0, 5), 5) select Run(p)).Max();
var part2 = (
    from p in GetPermutations(Range(5, 5), 5) select Run2(p)).Max();
Console.WriteLine((part1, part2, sw.Elapsed));
int Run(IEnumerable<int> phaseSettings)
{
    int next = 0;
    foreach (var i in phaseSettings)
    {
        next = IntCode.Run(program, i, next).Last();
    }

    return next;
}

int Run2(IEnumerable<int> phaseSettings)
{
    int next = 0;
    var amplifiers = Range(5, 5).ToDictionary(i => i, _ => new Amplifier(program));
    int iteration = 0;
    while (!amplifiers.Values.All(amp => amp.Halted))
    {
        iteration++;
        foreach (var i in phaseSettings)
        {
            next = iteration switch
            {
                1 => amplifiers[i].Run(i, next).FirstOrDefault() ?? throw new Exception(),
                _ => amplifiers[i].Run(next).FirstOrDefault() ?? amplifiers[i].Output
            };
        }
    }

    return next;
}

IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length) => length == 1 ?
    from t in list
    select Repeat(t, 1) : GetPermutations(list, length - 1).SelectMany(t => list.Where(e => !t.Contains(e)), (t1, t2) => t1.Concat(Repeat(t2, 1)).ToArray());