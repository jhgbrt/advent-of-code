using static System.Math;

namespace AdventOfCode.Year2021.Day19;

public class AoC202119
{
    public AoC202119() : this(Read.InputLines())
    {
    }
    public AoC202119(string[] input)
    {
        scanners = Align(
            from s in CreateScanners(input)
            from t in s.Transformations()
            select t);
    }
    ImmutableList<Scanner> scanners;

    public object Part1() => (from s in scanners from b in s.OffsetBeacons select b).Distinct().Count();

    public object Part2() => (from s1 in scanners
                              from s2 in scanners
                              select s1.offset.Distance(s2.offset)).Max();
    static ImmutableList<Scanner> Align(IEnumerable<Scanner> scanners)
    {
        var remaining = (
            from scanner in scanners.Skip(1)
            group scanner by scanner.id
        ).ToImmutableDictionary(g => g.Key);

        var found = ImmutableDictionary<int, Scanner>.Empty.Add(0, scanners.First());

        var q = ImmutableQueue<int>.Empty.Enqueue(0);

        while (q.Any() && remaining.Any())
        {
            q = q.Dequeue(out int id);
            foreach (var scanner in found[id].FindScannersInRange(remaining.Values))
            {
                //Console.WriteLine($"{id} - {scanner.id} ({remaining.Count()} remaining)");
                found = found.SetItem(scanner.id, scanner);
                q = q.Enqueue(scanner.id);
                remaining = remaining.Remove(scanner.id);
            }
        }
        return found.Values.ToImmutableList();
    }

    static IEnumerable<Scanner> CreateScanners(IEnumerable<string> input)
    {
        var enumerator = input.GetEnumerator();

        while (enumerator.MoveNext())
        {
            var id = int.Parse(enumerator.Current.Split(' ')[2]);
            yield return new Scanner(id, ReadPoints(enumerator).ToImmutableHashSet(), default);
        }
    }
    static IEnumerable<P> ReadPoints(IEnumerator<string> enumerator)
    {
        while (enumerator.MoveNext() && enumerator.Current != string.Empty)
        {
            var s = enumerator.Current.Split(',').Select(int.Parse).ToArray();
            yield return new P(s[0], s[1], s[2]);
        }
    }

}

record struct P(int x, int y, int z)
{
    public static P operator -(P left, P right)
      => new(left.x - right.x, left.y - right.y, left.z - right.z);

    public static P operator +(P left, P right)
      => new(left.x + right.x, left.y + right.y, left.z + right.z);

    public int Distance(P other)
      => Abs(other.x - x) + Abs(other.y - y) + Abs(other.z - z);
};

record Scanner(int id, ImmutableHashSet<P> beacons, P offset = default)
{
    private ImmutableHashSet<P>? _offsets;
    public ImmutableHashSet<P> OffsetBeacons
    {
        get
        {
            if (_offsets == null)
            {
                _offsets = beacons.Select(v => v + offset).ToImmutableHashSet();
            }
            return _offsets;
        }
    }


    static Func<P, P>[] TransformationFunctions =
        [
            v => v,
            v => new(v.x, -v.z, v.y),
            v => new(v.x, -v.y, -v.z),
            v => new(v.x, v.z, -v.y),
            v => new(-v.y, v.x, v.z),
            v => new(v.z, v.x, v.y),
            v => new(v.y, v.x, -v.z),
            v => new(-v.z, v.x, -v.y),
            v => new(-v.x, -v.y, v.z),
            v => new(-v.x, -v.z, -v.y),
            v => new(-v.x, v.y, -v.z),
            v => new(-v.x, v.z, v.y),
            v => new(v.y, -v.x, v.z),
            v => new(v.z, -v.x, -v.y),
            v => new(-v.y, -v.x, -v.z),
            v => new(-v.z, -v.x, v.y),
            v => new(-v.z, v.y, v.x),
            v => new(v.y, v.z, v.x),
            v => new(v.z, -v.y, v.x),
            v => new(-v.y, -v.z, v.x),
            v => new(-v.z, -v.y, -v.x),
            v => new(-v.y, v.z, -v.x),
            v => new(v.z, v.y, -v.x),
            v => new(v.y, -v.z, -v.x)
        ];

      internal IEnumerable<Scanner> Transformations()
        => from f in TransformationFunctions
           select new Scanner(id, beacons.Select(f).ToImmutableHashSet(), default);

    internal IEnumerable<Scanner> FindScannersInRange(IEnumerable<IEnumerable<Scanner>> scanners)
        => from s in scanners
           let t = (from scanner in s
                    from a in scanner.InRange(this)
                    select a).FirstOrDefault()
           where t is not null
           select t;

    private IEnumerable<Scanner> InRange(Scanner target)
        => from a in target.OffsetBeacons
           from r in beacons
           let aligned = this with { offset = a - r }
           where target.OffsetBeacons.Intersect(aligned.OffsetBeacons).CountIsAtLeast(12)
           select aligned;
}
