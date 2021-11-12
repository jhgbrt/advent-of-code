using Plumber;
using static AoC;
Console.WriteLine(Part1());
Console.WriteLine(Part2());

partial class AoC
{
    static string[] input = File.ReadAllLines("input.txt");
    static IEnumerable<(int vertex1, int vertex2)>? edges = (
                from line in File.ReadLines("input.txt")
                let parts = line.Split("<->").Select(s => s.Trim()).ToArray()
                let vertex1 = int.Parse(parts[0])
                from vertex2 in parts[1].Split(',').Select(int.Parse)
                select (vertex1: vertex1, vertex2: vertex2)
            ).ToArray();
    internal static Result Part1() => Run(() => new Graph(edges).Count(0));
    internal static Result Part2() => Run(() => new Graph(edges).SubGraphs().Count);


}

