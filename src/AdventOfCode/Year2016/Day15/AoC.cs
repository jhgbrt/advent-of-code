namespace AdventOfCode.Year2016.Day15;

public class AoC201615
{
    static Regex AoC201615Regex = new(@"^Disc #(?<id>\d+) has (?<positions>\d+) positions; at time=0, it is at position (?<position>\d+)\.$");
    static string[] input = Read.InputLines();
    static ImmutableArray<Disc> discs = (from line in input
                                         select AoC201615Regex.As<Disc>(line)).ToImmutableArray();

    static ImmutableArray<Disc> discs2 = discs.Add(new Disc(discs.Max(d => d.id) + 1, 11, 0));
    public int Part1() => Range(0, int.MaxValue).First(t => discs.All(d => d.Position(t) == 0));
    public int Part2() => Range(0, int.MaxValue).First(t => discs2.All(d => d.Position(t) == 0));
}

readonly record struct Disc(int id, int positions, int position)
{
    public int Position(int time) => (time + position + id) % positions;
}