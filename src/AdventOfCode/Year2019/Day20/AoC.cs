namespace AdventOfCode.Year2019.Day20;

public class AoC201920
{
    internal static string[] input = Read.SampleLines(1);

    public object Part1()
    {

        var letters = (
            from r in input.Index()
            from c in r.Item.Index()
            where char.IsLetter(c.Item)
            select (pos: new Coordinate(c.Index, r.Index), value: c.Item)
        );


        var ports = from a in letters
                    from b in letters
                    where a.pos.S == b.pos || a.pos.E == b.pos
                    let pos = 
                        a.pos.E == b.pos
                        ? a.pos.x == 0 ? b.pos.E : a.pos.W
                        : a.pos.y == 0 ? b.pos.S : a.pos.N
                    select (pos, $"{a.value}{b.value}");

        foreach (var p in ports) Console.WriteLine(p);



        var lines = (
            from line in input[2..^2]
            let chars = from c in line[2..^2]
                        select char.IsLetter(c) ? ' ' : c
            select new string(chars.ToArray())
            )
            .ToArray();

        var grid = new FiniteGrid(lines);


        Console.WriteLine(grid);
        return 0;
    }
    public object Part2() => "";

}

readonly record struct Coordinate(int x, int y)
{
    public static Coordinate Origin = new(0, 0);
    public int ManhattanDistance(Coordinate o) => Abs(x - o.x) + Abs(y - o.y);
    public override string ToString() => $"({x},{y})";
    public Coordinate S => this with { y = y + 1 };
    public Coordinate E => this with { x = x + 1 };
    public Coordinate W => this with { x = x - 1 };
    public Coordinate N => this with { y = y - 1 };
}
