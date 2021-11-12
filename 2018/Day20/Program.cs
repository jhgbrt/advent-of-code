using static AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

partial class AoC
{
    static string[] input = File.ReadAllLines("input.txt");

    internal static Result Part1() => Run(() => Part1(input));
    internal static Result Part2() => Run(() => Part2(input));
    public static int Part1(string[] input) => input.Single().Distances().Max();

    public static int Part2(string[] input) => input.Single().Distances().Where(i => i >= 1000).Count();
}
static class Ex
{
    public static IEnumerable<int> Distances(this string route)
    {
        (int x, int y) Next(char c, (int x, int y) p)
        {
            switch (c)
            {
                case 'N': return (p.x, p.y - 1);
                case 'S': return (p.x, p.y + 1);
                case 'E': return (p.x + 1, p.y);
                case 'W': return (p.x - 1, p.y);
            }
            throw new ArgumentOutOfRangeException(nameof(c));
        }

        (int x, int y) current = (0, 0);
        var positions = new Stack<(int x, int y)>();
        var distances = new Dictionary<(int x, int y), int>();
        foreach (var c in route[1..^1])
        {
            switch (c)
            {
                case '(':
                    positions.Push(current);
                    break;
                case ')':
                    current = positions.Pop();
                    break;
                case '|':
                    current = positions.Peek();
                    break;
                default:
                    var next = Next(c, current);
                    var distance = distances.ContainsKey(current) ? distances[current] + 1 : 1;
                    distances[next] = distances.ContainsKey(next) ? Math.Min(distance, distances[next]) : distance;
                    current = next;
                    break;
            }
        }

        return distances.Values;
    }
}