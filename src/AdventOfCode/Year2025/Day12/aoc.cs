using System.Runtime.CompilerServices;

namespace AdventOfCode.Year2025.Day12;

// Extension methods for int to handle bit operations on a 3x3 grid.
// The grid is represented as a bitmask where each bit corresponds to a cell.
// Bit position is calculated as y * size + x, with size=3.
static class Extensions
{
    extension(int source)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool BitIsSet(int index) => (source & 1 << index) != 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int SetBit(int index) => source | 1 << index;
    }
}
readonly record struct Coordinate(int x, int y)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly int GetIndex(int offset) => y * offset + x;

    public static Coordinate operator+(Coordinate left, Coordinate right) => new(left.x + right.x, left.y + right.y);
}

// Represents a 3x3 grid using a bitmask, normalized to its bounding box.
// Width and Height are the actual dimensions after normalization.
class Grid(int mask, int width, int height)
{
    public int Mask => mask;
    public int Width => width;
    public int Height => height;

    private const int Size = 3; // Fixed 3x3 grid size
    // Maps for rotating and flipping the grid.
    // RotateMap: rotates 90 degrees clockwise.
    // FlipMap: flips horizontally.
    private static readonly int[] RotateMap = [2, 5, 8, 1, 4, 7, 0, 3, 6];
    private static readonly int[] FlipMap = [2, 1, 0, 5, 4, 3, 8, 7, 6];
    
    
    public static Grid Parse(ReadOnlySpan<string> lines)
    {
        var mask = 0;
        for (var y = 0; y < lines.Length; y++)
        {
            for (var x = 0; x < lines[y].Length; x++)
            {
                if (lines[y][x] == '#')
                {
                    mask = mask.SetBit(new Coordinate(x,y).GetIndex(3)); // Set bit for '#' cells
                }
            }
        }

        return FromMask(mask); // Normalize the grid
    }

    internal bool this[Coordinate c] => Mask.BitIsSet(c.GetIndex(3)); // Accessor for cell at coordinate

    public int Area => BitOperations.PopCount((uint)Mask); // Count of set bits (filled cells)

    public IEnumerable<Coordinate> Cells => field ??= [.. Range(0, Size * Size)
            .Where(i => Mask.BitIsSet(i))
            .Select(i => new Coordinate(i % Size, i / Size))];

    private static Grid FromMask(int mask)
    {
        var (min, max) = (new Coordinate(Size, Size), new Coordinate(0, 0));

        // Find bounding box of filled cells
        for (var y = 0; y < Size; y++)
        {
            for (var x = 0; x < Size; x++)
            {
                if (!mask.BitIsSet(new Coordinate(x, y).GetIndex(3)))
                {
                    continue;
                }
                (min, max) = (new(int.Min(min.x, x), int.Min(min.y, y)), new(int.Max(max.x, x), int.Max(max.y, y)));
            }
        }

        var normalized = 0;
        // Shift the grid to start from (0,0) in the bounding box
        for (var y = 0; y < Size; y++)
        {
            for (var x = 0; x < Size; x++)
            {
                if (!mask.BitIsSet(new Coordinate(x, y).GetIndex(Size)))
                {
                    continue;
                }

                var nx = x - min.x;
                var ny = y - min.y;
                normalized = normalized.SetBit(new Coordinate(nx, ny).GetIndex(3));
            }
        }

        return new Grid(normalized, max.x - min.x + 1, max.y - min.y + 1);
    }

    // Permute the bits using the given map (for rotation/flip)
    private static int Permute(int mask, int[] map)
    {
        var result = 0;
        for (var i = 0; i < Size * Size; i++)
        {
            if (!mask.BitIsSet(i))
            {
                continue;
            }
            result = result.SetBit(map[i]); // Map bit i to map[i]
        }
        return result;
    }

    private bool Contains(int x, int y) => Mask.BitIsSet(new Coordinate(x, y).GetIndex(3));

    public Grid Rotate90() => FromMask(Permute(Mask, RotateMap)); // Rotate 90 degrees
    public Grid Flip() => FromMask(Permute(Mask, FlipMap)); // Flip horizontally

    public IEnumerable<Grid> Transformations => field ??= [.. GenerateTransformations()];
    private IEnumerable<Grid> GenerateTransformations()
    {
        var seen = new HashSet<int>(); // Track seen masks to avoid duplicates
        var g = this;
        // Generate all rotations and flips: 4 rotations, then flip, then 4 more rotations
        for (var mirror = 0; mirror < 2; mirror++)
        {
            for (var r = 0; r < 4; r++)
            {
                if (seen.Add(g.Mask)) // Add if not seen
                {
                    yield return g;
                }
                g = g.Rotate90(); // Rotate for next
            }
            g = g.Flip(); // Flip after 4 rotations
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                sb.Append(Contains(x, y) ? '#' : '.');
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }
}

// Represents a region specification: width x height, and counts of each present type.
class RegionSpec(int width, int height, int[] placements)
{
    public static RegionSpec Parse(ReadOnlySpan<char> line)
    {
        var ranges = new Range[7];
        var parts = line.Split(ranges, ' '); // Split into 7 parts: size + 6 counts

        var size = line[ranges[0]];
        var x = size.IndexOf('x');
        var width = int.Parse(size[..x]); // Width before 'x'
        var height = int.Parse(size[(x+1)..^1]); // Height after 'x', excluding trailing colon

        var placements = new int[6];
        for (int i = 0; i < 6; i++)
        {
            placements[i] = int.Parse(line[ranges[i + 1]]); // Parse each count
        }
        return new RegionSpec(width, height, placements);
    }

    // total area required by all placements
    private int RequiredArea(Grid[] presents) => placements.Zip(presents).Aggregate(0, (area, pair) => area + (pair.First * pair.Second.Area));

    private static readonly Dictionary<(int shapeIndex, int regionWidth, int regionHeight), Placement[]> cache = new();

    // a present to be placed, with its possible placements and area
    private readonly record struct Piece(IReadOnlyList<Placement> Placements, int Area) : IComparable<Piece>
    {
        public int CompareTo(Piece other)
        {
            // larger area first, then fewer placements first (heuristic for backtracking)
            var cmp = Area.CompareTo(other.Area);
            if (cmp != 0) return cmp;
            return other.Placements.Count.CompareTo(Placements.Count);
        }
    }
    // Represents a specific placement of a piece on the board
    // Mask is an array of ulongs representing occupied cells
    private readonly record struct Placement(IReadOnlyList<ulong> Mask)
    {
        internal static Placement Create(Coordinate position, Grid variant, int offset, int blockCount)
        {
            var mask = new ulong[blockCount]; // Placement mask
            foreach (var cell in variant.Cells)
            {
                var absolute = position + cell;
                var index = absolute.GetIndex(offset); // Linear index
                var block = index >> 6; // which ulong block. 6 because 2^6 = 64
                var bit = 1UL << (index & 63); // bit within block
                mask[block] |= bit; // Set bit in mask
            }
            
            return new Placement(mask);
        }
    }

    // Represents the board state during backtracking
    private class Board(int blockCount)
    {
        private readonly ulong[] mask = new ulong[blockCount];

        public bool Fits(Placement placement)
        {
            for (int i = 0; i < mask.Length; i++)
            {
                if ((mask[i] & placement.Mask[i]) != 0)
                {
                    return false;
                }
            }
            return true;
        }

        public void Apply(Placement placement)
        {
            for (int i = 0; i < mask.Length; i++)
            {
                mask[i] |= placement.Mask[i];
            }
        }

        public void Remove(Placement placement)
        {
            for (int i = 0; i < mask.Length; i++)
            {
                mask[i] ^= placement.Mask[i];
            }
        }
    }

    private Placement[] BuildPlacements(int shapeIndex, Grid grid, int blockCount)
    {
        var key = (shapeIndex, width, height);
        if (!cache.TryGetValue(key, out var placements))
        {
            placements = ComputePlacements(grid, blockCount);
            cache[key] = placements;
        }
        return placements;
    }

    private Placement[] ComputePlacements(Grid grid, int blockCount)
    {
        var placements = new List<Placement>();

        foreach (var variant in grid.Transformations)
        {
            if (variant.Width > width || variant.Height > height)
            {
                continue; // Skip if too big
            }

            // Try all top-left positions
            for (var y = 0; y <= height - variant.Height; y++)
            {
                for (var x = 0; x <= width - variant.Width; x++)
                {
                    placements.Add(Placement.Create(new(x, y), variant, width, blockCount));
                }
            }
        }

        return placements.ToArray();
    }

    private List<Piece>? BuildPieces(Grid[] presents, int blockCount)
    {
        var pieces = new List<Piece>();
        for (var shapeIndex = 0; shapeIndex < presents.Length; shapeIndex++)
        {
            var count = placements[shapeIndex];
            if (count == 0)
            {
                continue; // Skip if no placements needed
            }

            var shape = presents[shapeIndex];
            var piecePlacements = BuildPlacements(shapeIndex, shape, blockCount);
            if (placements.Length == 0)
            {
                return null; // Impossible to place this shape
            }

            // Add 'count' copies of this piece
            for (var i = 0; i < count; i++)
            {
                pieces.Add(new Piece(piecePlacements, shape.Area));
            }
        }

        pieces.Sort();

        return pieces;
    }

    private static bool SearchPieces(int pieceIndex, List<Piece> pieces, Board board, int currentArea, int[] cumulativeAreas, int totalCells)
    {
        if (pieceIndex == pieces.Count)
        {
            return true; // All pieces placed
        }

        // Pruning: if remaining area exceeds remaining cells
        if (currentArea + cumulativeAreas[pieceIndex] > totalCells)
        {
            return false;
        }

        var piece = pieces[pieceIndex];
        foreach (var placement in piece.Placements) // Try each possible placement
        {
            if (!board.Fits(placement)) // Check if placement fits (no overlap)
            {
                continue;
            }

            board.Apply(placement);

            if (SearchPieces(pieceIndex + 1, pieces, board, currentArea + piece.Area, cumulativeAreas, totalCells))
            {
                return true; // Success
            }

            board.Remove(placement); // Backtrack: remove the piece
        }

        return false; // No placement worked
    }

    public bool CanAccommodate(Grid[] presents, bool shortcut = true)
    {
        var total = width * height; // Total cells in region

        var required = RequiredArea(presents); // Total area needed

        if (shortcut)
        {
            int boundingArea = 9 * placements.Sum();
            if (total >= boundingArea)
            {
                return true;
            }
        }

        if (required > total)
        {
            return false;
        }

        if (total == 0)
        {
            return required == 0; 
        }

        // Compute number of blocks needed for the board
        // this works because we represent the board as an array of ulongs (64 bits each)
        // dividing by 64 gives us the number of ulongs needed to represent the board
        // we add 63 before dividing to round up
        var blockCount = (total + 63) / 64;
        var pieces = BuildPieces(presents, blockCount); // Build list of pieces to place
        if (pieces is null)
        {
            return false; // No placements possible
        }

        if (pieces.Count == 0)
        {
            return true; // No pieces needed
        }
        // Precompute cumulative areas for pruning
        var cumulativeAreas = new int[pieces.Count + 1];
        for (int i = pieces.Count - 1; i >= 0; i--)
        {
            cumulativeAreas[i] = cumulativeAreas[i + 1] + pieces[i].Area;
        }

        var board = new Board(blockCount); // Board as bitmask array
        return SearchPieces(0, pieces, board, 0, cumulativeAreas, total); // Start backtracking
    }
}

// Parsed input: 6 presents (grids) and list of specs.
class Input(Grid[] presents, RegionSpec[] specs)
{
    public Grid[] Presents => presents;
    public RegionSpec[] Specifications => specs;

    public static Input Parse(ReadOnlySpan<string> input)
    {
        var presents = new Grid[6];
        // Each present is 5 lines: header + 3 grid lines + empty line
        for (var i = 0; i < presents.Length; i++)
        {
            var lines = input[(i * 5)..(i * 5 + 4)][1..]; // Skip header, take 3 lines
            presents[i] = Grid.Parse(lines);
        }

        var specs = new RegionSpec[input.Length - 30]; // Specs start after 30 lines (6*5)
        for (var i = 0; i < specs.Length; i++)
        {
            specs[i] = RegionSpec.Parse(input[i + 30]);
        }

        return new Input(presents, specs);
    }
}

// shortcut is a fast solution based on the knowledge that shapes don't overlap.
// set to false for a 'correct' but slower solution
public class AoC202512(string[] lines, bool shortcut)
{
    public AoC202512() : this(Read.InputLines(), true) {}
    Input input = Input.Parse(lines);

    public int Part1() => input.Specifications.Count(spec => spec.CanAccommodate(input.Presents, shortcut));
    
    public int Part2() => -1; // There is no part 2 for day 12
}

// Unit tests
public class AoC202512Tests
{
    private readonly AoC202512 sut;
    private readonly ITestOutputHelper output;
    public AoC202512Tests(ITestOutputHelper output)
    {
        this.output = output;
        var input = Read.SampleLines();
        sut = new AoC202512(input, true);
    }

    [Fact]
    public void TestParsing()
    {
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(2, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(-1, sut.Part2());
    }

    [Fact]
    public void ValueAt_SetValueAt_Works()
    {
        var grid = Grid.Parse(
        [
            "#..",
            "...",
            "..#"
        ]);
        Assert.True(grid.Mask.BitIsSet(new Coordinate(0, 0).GetIndex(3)));
        Assert.False(grid.Mask.BitIsSet(new Coordinate(1, 1).GetIndex(3)));
        Assert.True(grid.Mask.BitIsSet(new Coordinate(2, 2).GetIndex(3)));
        var newMask = grid.Mask.SetBit(new Coordinate(1, 1).GetIndex(3));
    }

    [Fact]
    public void Grid_Transformations_Works()
    {
        var grid = Grid.Parse(
        [
            "##.",
            "#..",
            "#.#"
        ]);
        var transformations = grid.Transformations.ToArray();

        foreach ( var transformation in transformations )
        {
            output.WriteLine(transformation.ToString());
        }

        Assert.Equal(8, transformations.Length);
    }

    [Fact]
    public void Grid_Rotate90_Works()
    {
        var grid = Grid.Parse(
        [
            "##.",
            ".#.",
            "..#"
        ]);
        var rotated = grid.Rotate90();
        var expected = Grid.Parse(
        [
            "..#",
            ".##",
            "#.."
        ]);
        Assert.Equal(expected.Mask, rotated.Mask);
    }
    [Fact]
    public void Grid_Flip_Works()
    {
        var grid = Grid.Parse(
        [
            "##.",
            ".#.",
            "..#"
        ]);
        var flipped = grid.Flip();
        var expected = Grid.Parse(
        [
            ".##",
            ".#.",
            "#.."
        ]);
        Assert.Equal(expected.Mask, flipped.Mask);
    }

    [Fact]
    public void Grid_Area_Works()
    {
        var grid = Grid.Parse(
        [
            "#..",
            ".#.",
            "#.#"
        ]);
        Assert.Equal(4, grid.Area);
    }
}