namespace AdventOfCode.Year2017.Day12;

public class AoC201712
{
    static string[] input = Read.InputLines();
    static IEnumerable<(int vertex1, int vertex2)> edges = (
                from line in input
                let parts = line.Split("<->").Select(s => s.Trim()).ToArray()
                let vertex1 = int.Parse(parts[0])
                from vertex2 in parts[1].Split(',').Select(int.Parse)
                select (vertex1: vertex1, vertex2: vertex2)
            ).ToArray();
    public object Part1() => new Graph(edges).Count(0);
    public object Part2() => new Graph(edges).SubGraphs().Count;


}