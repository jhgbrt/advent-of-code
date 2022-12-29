using System.Collections;

namespace AdventOfCode.Year2020.Day20;

public class AoC202020
{
    static Regex TileRegex = new Regex(@"^Tile (?<Id>\d+):$");
    static IList<string> input = Read.InputLines();
    static ImmutableArray<TileData> tiledata = ReadTiles().ToImmutableArray();

    public long Part1() 
    {
        var tiles = tiledata.Select(t => new Part1Impl.Tile(t));
        var q = from tile in tiles
                let neighbors = (from n in tiles
                                 where tile.IsAdjacentTo(n)
                                 select n)
                where neighbors.Count() == 2
                select tile.Id;
        return q.Aggregate(1L, (x, y) => x * y);
    }
    public long Part2()
    {
        var tiles = tiledata.Select(t => new Part2Impl.Tile(t)).ToImmutableArray();
        var q = from tile in tiles
                let neighbors = (from n in tiles
                                 where tile.IsAdjacentTo(n)
                                 select n)
                where neighbors.Count() == 2
                select tile.Id;

        var part1 = q.Aggregate(1L, (x, y) => x * y);

        var pattern = new[]
        {
            "                  # " ,
            "#    ##    ##    ###" ,
            " #  #  #  #  #  #   "
        };
        var image = tiles.AssembleImage();
        var count = image.CountPattern(pattern);

        return image.Count('#') - count * 15;
    }

    static IEnumerable<TileData> ReadTiles()
    {
        var enumerator = input.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var id = int.Parse(TileRegex.Match(enumerator.Current).Groups["Id"].Value);
            var content = ReadLines(enumerator).TakeWhile(NotEmpty).ToArray();
            yield return new TileData(id, content);

        }
    }
    static bool NotEmpty(string s) => !string.IsNullOrEmpty(s);
    static IEnumerable<string> ReadLines(IEnumerator<string> enumerator)
    {
        while (enumerator.MoveNext()) yield return enumerator.Current;
    }
}

record TileData(int id, string[] content);

static class Part1Impl
{
    internal class Tile
    {
        public int Id { get; }
        public string[] Content { get; }
        public Tile(TileData data)
        {
            Id = data.id;
            Content = data.content;
            SideIds = new int[8];
            var bits = new[]
            {
            from c in Content[0] select c == '#',             // Top
            from l in Content select l[^1] == '#',            // Right
            from c in Content[^1].Reverse() select c == '#',  // Bottom
            from l in Content.Reverse() select l[0] == '#',   // Left
            from c in Content[0].Reverse() select c == '#',   // Top flipped
            from l in Content select l[0] == '#',             // Left flipped
            from c in Content[^1] select c == '#',            // Bottom flipped
            from l in Content.Reverse() select l[^1]=='#'     // Right flipped
        }.Select(x => new BitArray(x.ToArray())).ToArray();

            for (int i = 0; i < SideIds.Length; i++)
                bits[i].CopyTo(SideIds, i);
        }

        public bool IsAdjacentTo(Tile other) => other.Id != Id && SideIds.Any(id => other.SideIds.Contains(id));

        public int[] SideIds { get; }

        public override string ToString() => $"{Id} - {string.Join(" ", SideIds)}";
    }

}

static class Part2Impl
{
    internal record Edge(int cw, int ccw)
    {
        public bool IsCompatibleWith(Edge other) => Equals((cw, ccw), (other.ccw, other.cw));
    }

    internal class Tile
    {
        public static Tile Empty = new Tile(0, Array.Empty<char[]>());
        public int Id { get; }
        public char[][] Content { get; }
        public Tile(TileData data) : this(data.id, data.content) { }
        private Tile(int id, string[] content) : this(id, content.Select(s => s.ToCharArray()).ToArray()) { }
        private Tile(int id, char[][] content)
        {
            Id = id;
            Content = content;
            var sideIds = content.Any() ? new[]
            {
                from c in content[0] select c,             // Top clock-wise
                from l in content.Reverse() select l[0],   // Left clock-wise
                from c in content[^1].Reverse() select c,  // Bottom clock-wise
                from l in content select l[^1],            // Right clock-wise
                from c in content[0].Reverse() select c,   // Top counter-clock-wise
                from l in content select l[0],             // Left counter-clock-wise
                from c in content[^1] select c,            // Bottom counter-clock-wise
                from l in content.Reverse() select l[^1]   // Right counter-clock-wise
            }.Select(CreateInt).ToArray() : Array.Empty<int>();
            Sides = sideIds.Any() ? new Edge[]
            {
                new(sideIds[0], sideIds[4]),
                new(sideIds[1], sideIds[5]),
                new(sideIds[2], sideIds[6]),
                new(sideIds[3], sideIds[7]),
            } : new Edge[]
            {
                new(0,0),
                new(0,0),
                new(0,0),
                new(0,0)
            };
        }

        private int CreateInt(IEnumerable<char> chars)
        {
            var bools = chars.Select(c => c == '#').ToArray();
            if (bools.Length > 32) return 0;
            BitArray bits = new(bools);
            int[] ints = new[] { 0 };
            bits.CopyTo(ints, 0);
            return ints[0];
        }


        internal bool IsAdjacentTo(Tile other) => other.Id != Id && (HasCompatibleEdge(other) || HasCompatibleEdge(other.Flip()));
        internal bool IsRightFrom(Tile left) => left.Id != Id && (Sides.Any(left.Right.IsCompatibleWith) || Flip().Sides.Any(left.Right.IsCompatibleWith));
        internal bool IsBelow(Tile top) => top.Id != Id && (Sides.Any(top.Bottom.IsCompatibleWith) || Flip().Sides.Any(top.Bottom.IsCompatibleWith));
        internal bool HasCompatibleEdge(Tile other) => Sides.Any(s => other.Sides.Any(s.IsCompatibleWith));
        internal Edge Top => Sides[0];
        internal Edge Left => Sides[1];
        internal Edge Bottom => Sides[2];
        internal Edge Right => Sides[3];
        internal Edge[] Sides { get; }
        public override string ToString() => $"{Id}";

        public Tile Rotate() => new(Id, Rotate(Content).ToArray());
        static IEnumerable<char[]> Rotate(char[][] input)
        {
            for (int i = input.Length - 1; i >= 0; i--)
            {
                yield return input.Select(line => line[i]).ToArray();
            }
        }
        public Tile Flip() => new(Id, Content.Select(line => line.Reverse().ToArray()).ToArray());

        public override bool Equals(object? obj) => obj is Tile t && Equals(t);
        public bool Equals(Tile other) => Id == other.Id;
        public override int GetHashCode() => Id;

        public Tile AppendRight(Tile other) => new(0, Content.Zip(other.Content).Select(z => z.First.Concat(z.Second).ToArray()).ToArray());
        public Tile AppendBottom(Tile other) => new(0, Content.Concat(other.Content).ToArray());
        public Tile RemoveBorder() => new(Id, Content.Skip(1).Take(Content.Length - 2).Select(c => new string(c.Skip(1).Take(c.Length - 2).ToArray())).ToArray());
        private int CountPattern(ImmutableArray<(int x, int y)> coordinates)
        {
            var max = coordinates.Aggregate((p, q) => (Math.Max(p.x, q.x), Math.Max(p.y, q.y)));
            return (
                from y in Range(0, Content[0].Length - max.y)
                from x in Range(0, Content[0].Length - max.x)
                where coordinates.All(c => Content[y + c.y][x + c.x] == '#')
                select (x, y)
                ).Count();
        }
        public int Count(char value) => (from line in Content from c in line where c == value select c).Count();

        internal int CountPattern(string[] pattern)
        {
            var tile = this;
            var coordinates = (
                from y in Range(0, pattern.Length)
                from x in Range(0, pattern[0].Length)
                where pattern[y][x] == '#'
                select (x, y)).ToImmutableArray();

            var count = tile.CountPattern(coordinates);
            if (count == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    tile = tile.Rotate();
                    count = tile.CountPattern(coordinates);
                    if (count > 0) break;
                }
                tile = tile.Flip();
                for (int i = 0; i < 4; i++)
                {
                    tile = tile.Rotate();
                    count = tile.CountPattern(coordinates);
                    if (count > 0) break;
                }
            }
            return count;
        }

        public Tile AlignRight(Tile left)
        {
            var right = this;
            if (!right.IsRightFrom(left)) throw new Exception($"Tile {right} is is not positioned right from {left}, so can not be aligned");
            if (!right.HasCompatibleEdge(left)) right = right.Flip();
            while (!left.Right.IsCompatibleWith(right.Left)) right = right.Rotate();
            return right;
        }

        public Tile AlignBelow(Tile top)
        {
            var bottom = this;
            if (!bottom.IsBelow(top)) throw new Exception($"Tile {bottom} is is not positioned below {top}, so can not be aligned");
            if (!bottom.HasCompatibleEdge(top)) bottom = bottom.Flip();
            while (!top.Bottom.IsCompatibleWith(bottom.Top)) bottom = bottom.Rotate();
            return bottom;
        }

        public Tile MakeTopLeft(IEnumerable<Tile> neigbors)
        {
            var corner = this;
            if (!neigbors.All(n => corner.IsAdjacentTo(n))) throw new Exception($"Tile {corner} is not adjacent with all of {string.Join(";", neigbors.AsEnumerable())}, so can not be aligned");

            var commonEdges = (
                from side in Sides
                from n in neigbors
                from nside in n.Sides
                where side.IsCompatibleWith(nside)
                select side
                ).ToList();

            if (!corner.Sides.Any(s => commonEdges.Any(s.IsCompatibleWith)))
                corner = corner.Flip();

            while (!commonEdges.Any(corner.Right.IsCompatibleWith))
                corner = corner.Rotate();

            return corner;
        }
    }


    internal static Tile AssembleImage(this ImmutableArray<Tile> input)
    {
        var gridSize = (int)Math.Sqrt(input.Length);

        var all = (
            from tile in input
            let n = input.Where(tile.IsAdjacentTo).ToImmutableArray()
            select (tile.Id, tile, neighbors: n)
            ).ToImmutableDictionary(t => t.Id, t => (t.tile, t.neighbors));

        var neighborsById = (
            from tile in input
            let n = input.Where(tile.IsAdjacentTo).ToImmutableArray()
            select (tile.Id, tile, neighbors: n)
            ).ToImmutableDictionary(t => t.Id, t => t.neighbors);

        var (left, neighbors) = (
            from item in all
            where item.Value.neighbors.Length == 2
            select item
            ).First().Value;

        left = left.MakeTopLeft(neighbors);

        var image = new Tile[gridSize][];
        for (int i = 0; i < gridSize; i++)
            image[i] = new Tile[gridSize];
        for (int i = 0; i < gridSize; i++)
            for (int j = 0; j < gridSize; j++)
                image[i][j] = Tile.Empty;


        image[0][0] = left;

        {
            for (int y = 1; y < gridSize; y++)
            {
                var previous = image[y - 1][0];
                neighbors = neighborsById[previous.Id];
                var next = neighbors.Single(n => n.IsBelow(previous)).AlignBelow(previous);
                image[y][0] = next;
            }
            for (int y = 0; y < gridSize; y++)
            {
                for (int x = 1; x < gridSize; x++)
                {
                    var previous = image[y][x - 1];
                    neighbors = neighborsById[previous.Id];
                    var next = neighbors.Single(n => n.IsRightFrom(previous)).AlignRight(previous);
                    image[y][x] = next;
                }

            }
        }


        return image.ToTile();
    }


    internal static Tile ToTile(this IEnumerable<IEnumerable<Tile>> image)
        => image
                .Select(line => line.Select(t => t.RemoveBorder()).Aggregate((x, y) => x.AppendRight(y)))
                    .Aggregate((x, y) => x.AppendBottom(y));

    internal static string Visualize(this List<List<Tile>> image) => new StringBuilder()
            .Append(string.Join(Environment.NewLine + Environment.NewLine,
                    from line in image
                    select string.Join(Environment.NewLine,
                                       from i in Enumerable.Range(0, line[0].Content.Length)
                                       select string.Join(" ", from tile in line
                                                               select tile.Content[i]))))
            .ToString();

}