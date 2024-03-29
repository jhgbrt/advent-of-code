namespace AdventOfCode.Year2015.Day09;

public class AoC201509
{
    static string[] lines = Read.InputLines();

    public object Part1() => MinMax().min;
    public object Part2() => MinMax().max;

    static Regex regex = new(@"^(?<from>\w+) to (?<to>\w+) = (?<distance>\d+$)", RegexOptions.Compiled);

    static (int min, int max) MinMax()
    {
        var edges = from line in lines
                    let match = regex.Match(line)
                    let source = match.Groups["from"].Value
                    let target = match.Groups["to"].Value
                    let distance = int.Parse(match.Groups["distance"].Value)
                    select (source, target, distance);

        var vertices = edges.Select(e => e.source).Concat(edges.Select(e => e.target)).ToHashSet();

        var distances = edges.Concat(edges.Select(e => (source: e.target, target: e.source, e.distance))).ToDictionary(e => (e.source, e.target), e => e.distance);

        (var min, var max) = GetPermutations(vertices, vertices.Count)
            .Aggregate((min: int.MaxValue, max: int.MinValue), (x, p) =>
            {
                var distance = p.Zip(p.Skip(1)).ToList().Select(p => distances[(p.First, p.Second)]).Sum();
                return (x.min > distance ? distance : x.min, x.max < distance ? distance : x.max);
            });

        return (min, max);
    }
    static IEnumerable<T[]> GetPermutations<T>(IReadOnlyCollection<T> list, int length) => length == 1
            ? list.Select(t => new[] { t })
            : GetPermutations(list, length - 1).SelectMany(t => list.Where(e => !t.Contains(e)), (t1, t2) => t1.Concat(new[] { t2 }).ToArray());
}