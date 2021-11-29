using static AdventOfCode.Year2016.Day03.AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2016.Day03
{
    partial class AoC
    {
        static bool test = false;
        public static string[] input = File.ReadAllLines(test ? "sample.txt" : "input.txt");

        internal static Result Part1() => Run(() => (from triangle in GetTriangles()
                                                     where triangle.IsValid
                                                     select triangle).Count());
        internal static Result Part2() => Run(() => (from chunk in GetTriangles().Chunk(3)
                                                     from triangle in Transpose(chunk)
                                                     where triangle.IsValid
                                                     select triangle).Count());

        static IEnumerable<Triangle> Transpose(Triangle[] chunk)
        {
            yield return new(chunk[0].x, chunk[1].x, chunk[2].x);
            yield return new(chunk[0].y, chunk[1].y, chunk[2].y);
            yield return new(chunk[0].z, chunk[1].z, chunk[2].z);
        }

        static IEnumerable<Triangle> GetTriangles() => from line in input
                                                       select new Triangle(
                                                           int.Parse(line.Substring(2, 3).Trim()),
                                                           int.Parse(line.Substring(7, 3).Trim()),
                                                           int.Parse(line.Substring(12, 3).Trim())
                                                           );
    }
}
readonly record struct Triangle(int x, int y, int z)
{
    public bool IsValid => x + y > z && y + z > x && x + z > y;
}
