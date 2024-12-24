namespace AdventOfCode.Year2024.Day24;


public class AoC202424(string[] input)
{
    public AoC202424() : this(Read.InputLines()) {}

    Dictionary<string, int> wires = input
        .TakeWhile(l => !string.IsNullOrEmpty(l))
        .Select(line => line.Split(": ") switch
            {
                [string label, string n] v => (label, value: int.Parse(n)),
                _ => default
            })
        .ToDictionary(x => x.label, x => x.value);
    
    List<Gate> gates = input
        .SkipWhile(l => !string.IsNullOrEmpty(l))
        .Skip(1)
        .Select(Gate.Parse)
        .ToList();

    public ulong Part1()
    {
        var queue = new Queue<Gate>(gates);
        while (queue.Count > 0)
        {
            var gate = queue.Dequeue();
            if (wires.ContainsKey(gate.left) && wires.ContainsKey(gate.right))
            {
                wires[gate.output] = gate.Process(wires);
            }
            else
            {
                queue.Enqueue(gate);
            }
        }

        return wires.Keys
            .Where(k => k is ['z', ..])
            .OrderDescending()
            .Aggregate(0UL, (n, k) => (n << 1) | (uint)wires[k]);
    }

    public string Part2()
    {
        var maxZ = wires.Keys.Where(x => x[0] == 'z').MaxBy(x => int.Parse(x[1..]));

        // identifies the wrong gates
        // inspired by https://www.reddit.com/r/adventofcode/comments/1hl698z/comment/m3kt1je/
        var q = from g in gates
                where g switch
                {
                    (_, not '^', _, ['z', ..] output) 
                        => output != maxZ,
                    ([not ('x' or 'y' or 'z'), ..], 
                     '^', 
                     [not ('x' or 'y' or 'z'), ..], 
                     [not ('x' or 'y' or 'z'), ..])
                        => true,
                    (not "x00", '&', not "x00", _)
                        => (from s in gates
                            where s.IsConnectedTo(g) && s.@operator != '|'
                            select g).Any(),
                    (_, '^', _, _)
                        => (from s in gates
                            where s.IsConnectedTo(g) && s.@operator == '|'
                            select g).Any(),
                    _
                        => false
                }
                select g.output;

        var wrong = q.ToHashSet();

        return string.Join(",", wrong.OrderBy(x => x));

    }

}

public class AoC202424Tests(ITestOutputHelper output)
{

    [Fact]
    public void TestPart1()
    {
        var input = Read.SampleLines(1);
        var sut = new AoC202424(input);
        Assert.Equal(2024U, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        var input = Read.SampleLines(2);
        var sut = new AoC202424(input);
        Assert.Equal("z00,z01,z02,z03,z04,z05", sut.Part2());
    }
}



record struct Gate(string left, char @operator, string right, string output)
{
    public static Gate Parse(string line) => line.Split(' ') switch
    {
        [string left, string o, string right, _, string output]
            => new Gate(left, o switch
            {
                "AND" => '&',
                "OR" => '|',
                "XOR" => '^',
                _ => '\0'
            }, right, output),
            _ => default
    };
    public readonly int Process(Dictionary<string, int> wires) => @operator switch
    {
        '&' => wires[left] & wires[right],
        '|' => wires[left] | wires[right],
        '^' => wires[left] ^ wires[right],
        _ => throw new Exception()
    };
    public readonly bool IsConnectedTo(Gate other) => left == other.output || right == other.output;
}

