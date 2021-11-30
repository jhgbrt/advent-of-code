using static AdventOfCode.Year2020.Day20.Part1.AoC;
using System.Collections;
namespace AdventOfCode.Year2020.Day20.Part1;

public static class Runner
{
    public static object Run()
    {
        var input = ReadInput("input.txt").ToArray();

        var q = from tile in input
                let neighbors = (from n in input
                                 where tile.IsAdjacentTo(n)
                                 select n)
                where neighbors.Count() == 2
                select tile.Id;

        return q.Aggregate(1L, (x, y) => x * y);

    }
}



class Tile
{
    public int Id { get; }
    public string[] Content { get; }
    public Tile(int id, string[] content)
    {
        Id = id;
        Content = content;
        SideIds = new int[8];
        var bits = new[]
        {
            from c in content[0] select c == '#',             // Top
            from l in content select l[^1] == '#',            // Right
            from c in content[^1].Reverse() select c == '#',  // Bottom
            from l in content.Reverse() select l[0] == '#',   // Left
            from c in content[0].Reverse() select c == '#',   // Top flipped
            from l in content select l[0] == '#',             // Left flipped
            from c in content[^1] select c == '#',            // Bottom flipped
            from l in content.Reverse() select l[^1]=='#'     // Right flipped
        }.Select(x => new BitArray(x.ToArray())).ToArray();

        for (int i = 0; i < SideIds.Length; i++)
            bits[i].CopyTo(SideIds, i);
    }

    public bool IsAdjacentTo(Tile other) => other.Id != Id && SideIds.Any(id => other.SideIds.Contains(id));
   
    public int[] SideIds { get; }

    public override string ToString() => $"{Id} - {string.Join(" ", SideIds)}";
}

static class AoC
{

    internal static IEnumerable<Tile> ReadInput(string fileName)
    {
        var enumerator = Read.Lines(typeof(AoCImpl), fileName).GetEnumerator();
        foreach (var tile in ReadTiles(enumerator)) yield return tile;
    }

    static Regex TileRegex = new(@"^Tile (?<Id>\d+):$");
    static IEnumerable<Tile> ReadTiles(IEnumerator<string> enumerator)
    {
        while (enumerator.MoveNext())
        {
            var id = int.Parse(TileRegex.Match(enumerator.Current).Groups["Id"].Value);
            var content = ReadLines(enumerator).TakeWhile(s => !string.IsNullOrEmpty(s)).ToArray();
            yield return new Tile(id, content);

        }
    }
    static IEnumerable<string> ReadLines(IEnumerator<string> enumerator)
    {
        while (enumerator.MoveNext()) yield return enumerator.Current;
    }

}

public class Tests
{
    [Fact]
    public void Part1()
    {
        var input = ReadInput("example.txt").ToArray();
        Assert.Equal(9, input.Count());

        var q = from tile in input
                let neighbors = (from n in input
                                 where tile.IsAdjacentTo(n)
                                 select n)
                where neighbors.Count() == 2
                select (tile, neighbors);

        var result = q.Select(q => q.tile.Id).Aggregate(1L, (x, y) => x * y);

        Assert.Equal(20899048083289, result);
    }

}