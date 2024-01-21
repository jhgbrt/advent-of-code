namespace AdventOfCode.Year2019.Day07;

public class AoC201907(string[] input)
{
    public AoC201907() : this(Read.InputLines())
    {
        
    }

    internal ImmutableArray<int> program = input[0].Split(',').Select(int.Parse).ToImmutableArray();

    public object Part1() => (
        from p in GetPermutations(Range(0, 5), 5)
        select Run(p)
        ).Max();

    long Run(IEnumerable<int> phaseSettings)
    {
        long next = 0;
        foreach (var i in phaseSettings)
        {
            next = new IntCode(program.ToArray()).Run(new[] { i, next }).Last();
        }
        return next;
    }

    public object Part2() => (
        from p in GetPermutations(Range(5, 5), 5)
        select Run2(p)
        ).Max();

    long Run2(IEnumerable<int> phaseSettings)
    {
        long next = 0;

        var amplifiers = Range(5, 5).ToDictionary(i => i, _ => new IntCode(program));

        int iteration = 0;

        while (!amplifiers.Values.All(amp => amp.Halted))
        {
            iteration++;
            foreach (var i in phaseSettings)
            {
                next = iteration switch
                {
                    1 => amplifiers[i].Run(new[] { i, next }).First(),
                    _ => amplifiers[i].Run(next) ?? amplifiers[i].LastOutput
                };
            }
        }

        return next;
    }


    internal static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length) => length == 1
           ? from t in list select Repeat(t, 1)
           : GetPermutations(list, length - 1).SelectMany(t => list.Where(e => !t.Contains(e)), (t1, t2) => t1.Concat(Repeat(t2, 1)).ToArray());

}

public class AoC201907Tests
{
    AoC201907 sut = new AoC201907(Read.SampleLines());
    [Fact]
    public void TestPart1()
    {
        Assert.Equal(199988L, sut.Part1());
    }
    [Fact]
    public void TestPart2()
    {
        Assert.Equal(17519904L, sut.Part2());
    }

}