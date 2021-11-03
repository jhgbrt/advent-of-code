var test = false;

var filename = test ? "sample.txt" : "input.txt";

var lines = File.ReadAllLines(filename);

var grid = new Grid((
    from y in Enumerable.Range(0, lines.Length)
    from x in Enumerable.Range(0, lines[y].Length)
    where lines[y][x] == '#'
    select new Coordinate(x, y)
    ).ToHashSet(), lines.Length);

//Console.WriteLine(grid);

//for (int i = 0; i < 4; i++)
//{
//    grid = grid.Next();
//    Console.WriteLine(grid);
//}

Console.WriteLine(Part1(grid));
Console.WriteLine(Part2(grid));

int Part1(Grid grid) => Enumerable.Range(0, 100).Aggregate(grid, (g, i) => g.Next1()).Count();
int Part2(Grid grid) => Enumerable.Range(0, 100).Aggregate(grid, (g, i) => g.Next2()).Count();


record Coordinate(int x, int y)
{
    public override string ToString() => $"({x},{y})";
}