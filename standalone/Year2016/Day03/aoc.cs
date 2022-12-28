var input = File.ReadAllLines("input.txt");
var sw = Stopwatch.StartNew();
var part1 = (
    from triangle in GetTriangles()
    where triangle.IsValid
    select triangle).Count();
var part2 = (
    from chunk in GetTriangles().Chunk(3)
    from triangle in Transpose(chunk)
    where triangle.IsValid
    select triangle).Count();
Console.WriteLine((part1, part2, sw.Elapsed));
IEnumerable<Triangle> Transpose(Triangle[] chunk)
{
    yield return new(chunk[0].x, chunk[1].x, chunk[2].x);
    yield return new(chunk[0].y, chunk[1].y, chunk[2].y);
    yield return new(chunk[0].z, chunk[1].z, chunk[2].z);
}

IEnumerable<Triangle> GetTriangles() =>
    from line in input
    select new Triangle(int.Parse(line.Substring(2, 3).Trim()), int.Parse(line.Substring(7, 3).Trim()), int.Parse(line.Substring(12, 3).Trim()));
readonly record struct Triangle(int x, int y, int z)
{
    public bool IsValid => x + y > z && y + z > x && x + z > y;
}