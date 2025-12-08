namespace AdventOfCode.Year2025.Day08;

record struct Coordinate(int X, int Y, int Z) 
{
    public static Coordinate Parse(ReadOnlySpan<char> s)
    {
        var parts = new Range[3];
        s.Split(parts, ',');
        return new Coordinate(
            int.Parse(s[parts[0]]),
            int.Parse(s[parts[1]]),
            int.Parse(s[parts[2]])
        );
    }
    public double DistanceSquared(Coordinate other) => Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2) + Math.Pow(Z - other.Z, 2);
}

record Circuit(HashSet<Coordinate> Members)
{
    public int Size => Members.Count;
    
    public bool Contains(Coordinate coord) => Members.Contains(coord);
    
    public Circuit MergeWith(Circuit other)
    {
        var merged = new HashSet<Coordinate>(Members);
        merged.UnionWith(other.Members);
        return new Circuit(merged);
    }
}
class CircuitManager
{
    private Dictionary<Coordinate, Circuit> coordinateToCircuit = [];

    public CircuitManager(Coordinate[] coordinates)
    {
        foreach (var coord in coordinates)
        {
            coordinateToCircuit[coord] = new Circuit([coord]);
        }
    }

    public Circuit FindCircuit(Coordinate coord) => coordinateToCircuit[coord];

    public bool Merge(Coordinate c1, Coordinate c2)
    {
        var circuit1 = FindCircuit(c1);
        var circuit2 = FindCircuit(c2);

        if (circuit1 == circuit2)
            return false;

        var merged = circuit1.MergeWith(circuit2);
        foreach (var coord in merged.Members)
        {
            coordinateToCircuit[coord] = merged;
        }

        return true;
    }

    public IEnumerable<Circuit> GetUniqueCircuits() =>
        coordinateToCircuit.Values.Distinct();
}


public class AoC202508
{
    public AoC202508(string[] input, int iterations, TextWriter writer)
    {
        this.writer = writer;
        this.iterations = iterations;
        this.coordinates = input.Select(line => Coordinate.Parse(line)).ToArray();
        this.pairs = (
            from i in Enumerable.Range(0, coordinates.Length)
            from j in Enumerable.Range(i + 1, coordinates.Length - i - 1)
            orderby coordinates[i].DistanceSquared(coordinates[j])
            select (coordinates[i], coordinates[j])
            ).ToArray();
        this.circuitManager = new CircuitManager(coordinates);
    }

    public AoC202508() : this(Read.InputLines(), 1000, Console.Out) { }
    private readonly TextWriter writer;
    private readonly Coordinate[] coordinates;
    private readonly (Coordinate c1, Coordinate c2)[] pairs;
    private readonly int iterations;
    private readonly CircuitManager circuitManager;
    public int Part1()
    {        
        foreach (var (c1, c2) in pairs[..iterations])
        {
            circuitManager.Merge(c1, c2);
        }

        return circuitManager.GetUniqueCircuits()
            .OrderByDescending(c => c.Size)
            .Take(3)
            .Aggregate(1, (acc, circuit) => acc * circuit.Size);
    }

    public long Part2()
    {
        bool count = true;
        int n = 0;
        (Coordinate c1, Coordinate c2) last = default;
        foreach (var (c1, c2) in pairs[iterations..])
        {
            if (circuitManager.Merge(c1, c2))
            {
                count = false;
                last = (c1, c2);
                var uniqueCircuits = circuitManager.GetUniqueCircuits().Count();
                if (uniqueCircuits == 1)
                    break;
            }
            else 
            {
                if (count)
                    n++;
            }
        }
        writer.WriteLine(n);
        return 1L * last.c1.X * last.c2.X;
    }
}

public class AoC202508Tests
{
    private readonly AoC202508 sut;
    public AoC202508Tests(ITestOutputHelper output)
    {
        var input = Read.SampleLines();
        sut = new AoC202508(input, 10, new TestWriter(output));
    }

    [Fact]
    public void TestParsing()
    {
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(40, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        sut.Part1(); // we need to run part 1 first to setup the state
        Assert.Equal(25272, sut.Part2());
    }

}