namespace AdventOfCode.Year2019.Day24;

public class AoC201924
{
    internal readonly Grid initialState;
    
    public AoC201924(string[] input)
    {
        initialState = Grid.Parse(input);
    }
    
    public AoC201924(Grid initialState)
    {
        this.initialState = initialState;
    }
    
    public AoC201924() : this(Read.InputLines())
    {
    }
    
    public int Part1()
    {
        var seen = new HashSet<Grid>();
        var current = initialState;
        
        while (!seen.Contains(current))
        {
            seen.Add(current);
            current = current.Evolve();
        }
        
        return current.BiodiversityRating;
    }
    
    public int Part2() => Part2(200);
    
    public int Part2(int minutes)
    {
        // For Part 2, use the initial state (which should be parsed with recursive mode)
        var levels = new Dictionary<int, Grid> { [0] = initialState };
        
        for (int minute = 0; minute < minutes; minute++)
        {
            levels = EvolveRecursive(levels);
        }
        
        return levels.Values.Sum(g => g.BugCount);
    }
    
    static Dictionary<int, Grid> EvolveRecursive(Dictionary<int, Grid> levels)
    {
        int minLevel = levels.Keys.Min() - 1;
        int maxLevel = levels.Keys.Max() + 1;
        
        var nextLevels = new Dictionary<int, Grid>();
        
        for (int level = minLevel; level <= maxLevel; level++)
        {
            var currentGrid = levels.GetValueOrDefault(level, Grid.Empty);
            var innerGrid = levels.GetValueOrDefault(level + 1, Grid.Empty);
            var outerGrid = levels.GetValueOrDefault(level - 1, Grid.Empty);
            
            var nextGrid = currentGrid.EvolveRecursive(innerGrid, outerGrid);
            
            if (!nextGrid.IsEmpty)
            {
                nextLevels[level] = nextGrid;
            }
        }
        
        return nextLevels;
    }
}

public readonly record struct Position(int X, int Y)
{
    public static readonly Position[] Directions = 
    [
        new(0, -1),  // North
        new(1, 0),   // East
        new(0, 1),   // South
        new(-1, 0)   // West
    ];
    
    public Position Move(Position direction) => new(X + direction.X, Y + direction.Y);
    
    public IEnumerable<Position> Neighbors() => Directions.Select(Move);
}

public readonly record struct Grid
{
    const int Size = 5;
    const int Center = 2;
    readonly int bits;
    
    Grid(int bits) => this.bits = bits;
    
    public int BiodiversityRating => bits;
    public bool IsEmpty => bits == 0;
    public int BugCount => BitOperations.PopCount((uint)bits);
    
    public static Grid Empty => new(0);
    
    public static Grid Parse(string[] lines, bool recursive = false)
    {
        int state = 0;
        
        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                if (recursive && x == Center && y == Center) continue; // Center is always empty in recursive mode
                
                if (lines[y][x] == '#')
                {
                    state |= (1 << (y * Size + x));
                }
            }
        }
        
        return new Grid(state);
    }
    
    public bool HasBug(Position pos, bool recursive = false)
    {
        if (!IsValid(pos)) return false;
        if (recursive && pos.X == Center && pos.Y == Center) return false; // Center is always empty in recursive
        int index = pos.Y * Size + pos.X;
        return (bits & (1 << index)) != 0;
    }
    
    static bool IsValid(Position pos) =>
        pos.X >= 0 && pos.X < Size && pos.Y >= 0 && pos.Y < Size;
    
    public Grid Evolve()
    {
        int nextBits = 0;
        
        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                var pos = new Position(x, y);
                int adjacentBugs = CountAdjacentBugs(pos);
                
                bool willHaveBug = HasBug(pos)
                    ? adjacentBugs == 1
                    : adjacentBugs is 1 or 2;
                
                if (willHaveBug)
                {
                    nextBits |= (1 << (y * Size + x));
                }
            }
        }
        
        return new Grid(nextBits);
    }
    
    int CountAdjacentBugs(Position pos)
    {
        int count = 0;
        foreach (var neighbor in pos.Neighbors())
        {
            if (HasBug(neighbor)) count++;
        }
        return count;
    }
    
    public Grid EvolveRecursive(Grid inner, Grid outer)
    {
        int nextBits = 0;
        
        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                if (x == Center && y == Center) continue;
                
                var pos = new Position(x, y);
                int adjacentBugs = CountRecursiveNeighbors(pos, inner, outer);
                
                bool willHaveBug = HasBug(pos)
                    ? adjacentBugs == 1
                    : adjacentBugs is 1 or 2;
                
                if (willHaveBug)
                {
                    nextBits |= (1 << (y * Size + x));
                }
            }
        }
        
        return new Grid(nextBits);
    }
    
    int CountRecursiveNeighbors(Position pos, Grid inner, Grid outer)
    {
        int count = 0;
        
        foreach (var dir in Position.Directions)
        {
            var neighbor = pos.Move(dir);
            
            // Check if neighbor is the center - if so, count edge of inner grid
            if (neighbor.X == Center && neighbor.Y == Center)
            {
                count += CountInnerEdge(dir, inner);
            }
            // Check if neighbor is off the grid - if so, check outer grid
            else if (!IsValid(neighbor))
            {
                count += CountOuterTile(dir, outer);
            }
            // Normal neighbor on same level
            else if (HasBug(neighbor, recursive: true))
            {
                count++;
            }
        }
        
        return count;
    }
    
    static int CountInnerEdge(Position direction, Grid inner)
    {
        int count = 0;
        
        // North: count bottom row of inner
        if (direction.Y == -1)
        {
            for (int x = 0; x < Size; x++)
                if (inner.HasBug(new Position(x, Size - 1), recursive: true)) count++;
        }
        // South: count top row of inner
        else if (direction.Y == 1)
        {
            for (int x = 0; x < Size; x++)
                if (inner.HasBug(new Position(x, 0), recursive: true)) count++;
        }
        // West: count right column of inner
        else if (direction.X == -1)
        {
            for (int y = 0; y < Size; y++)
                if (inner.HasBug(new Position(Size - 1, y), recursive: true)) count++;
        }
        // East: count left column of inner
        else if (direction.X == 1)
        {
            for (int y = 0; y < Size; y++)
                if (inner.HasBug(new Position(0, y), recursive: true)) count++;
        }
        
        return count;
    }
    
    static int CountOuterTile(Position direction, Grid outer)
    {
        // Determine which tile in the outer grid to check
        Position outerPos = direction switch
        {
            { Y: -1 } => new Position(Center, Center - 1), // North edge -> tile above center
            { Y: 1 }  => new Position(Center, Center + 1), // South edge -> tile below center
            { X: -1 } => new Position(Center - 1, Center), // West edge -> tile left of center
            { X: 1 }  => new Position(Center + 1, Center), // East edge -> tile right of center
            _ => throw new InvalidOperationException()
        };
        
        return outer.HasBug(outerPos, recursive: true) ? 1 : 0;
    }
    
    public override string ToString() => ToString(false);
    
    public string ToString(bool recursive)
    {
        var sb = new StringBuilder();
        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                if (recursive && x == Center && y == Center)
                    sb.Append('?');
                else
                    sb.Append(HasBug(new Position(x, y), recursive) ? '#' : '.');
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }
}

public class AoC201924Tests
{
    readonly ITestOutputHelper output;
    
    public AoC201924Tests(ITestOutputHelper output)
    {
        this.output = output;
    }
    
    [Fact]
    public void TestSample()
    {
        var input = Read.SampleLines();
        var sut = new AoC201924(input);
        var result = sut.Part1();
        
        output.WriteLine($"Initial state:");
        output.WriteLine(sut.initialState.ToString());
        output.WriteLine($"Biodiversity rating: {result}");
        
        Assert.Equal(2129920, result);
    }
    
    [Fact]
    public void TestPart1()
    {
        var sut = new AoC201924();
        var result = sut.Part1();
        Assert.Equal(25719471, result);
    }
    
    [Fact]
    public void TestEvolution()
    {
        var input = Read.SampleLines();
        var sut = new AoC201924(input);
        
        var state = sut.initialState;
        output.WriteLine("Initial state:");
        output.WriteLine(state.ToString());
        
        for (int i = 1; i <= 4; i++)
        {
            state = state.Evolve();
            output.WriteLine($"After {i} minute(s):");
            output.WriteLine(state.ToString());
        }
    }
    
    [Fact]
    public void TestPosition()
    {
        var pos = new Position(2, 2);
        var neighbors = pos.Neighbors().ToList();
        
        Assert.Equal(4, neighbors.Count);
        Assert.Contains(new Position(2, 1), neighbors); // North
        Assert.Contains(new Position(3, 2), neighbors); // East
        Assert.Contains(new Position(2, 3), neighbors); // South
        Assert.Contains(new Position(1, 2), neighbors); // West
    }
    
    [Fact]
    public void TestGridParsing()
    {
        var input = new[] 
        {
            "....#",
            "#..#.",
            "#..##",
            "..#..",
            "#...."
        };
        
        var grid = Grid.Parse(input);
        
        Assert.True(grid.HasBug(new Position(4, 0)));
        Assert.True(grid.HasBug(new Position(0, 1)));
        Assert.False(grid.HasBug(new Position(0, 0)));
        Assert.False(grid.HasBug(new Position(1, 0)));
    }
    
    [Fact]
    public void TestBiodiversityCalculation()
    {
        // From puzzle: positions 15 (32768) and 21 (2097152) = 2129920
        var input = new[] 
        {
            ".....",
            ".....",
            ".....",
            "#....",
            ".#..."
        };
        
        var grid = Grid.Parse(input);
        
        // Position 15 = row 3, col 0 = 2^15 = 32768
        // Position 21 = row 4, col 1 = 2^21 = 2097152
        Assert.Equal(2129920, grid.BiodiversityRating);
    }
    
    [Fact]
    public void TestPart2Sample()
    {
        var input = Read.SampleLines();
        var sut = new AoC201924(Grid.Parse(input, recursive: true));
        
        var result = sut.Part2(10);
        output.WriteLine($"After 10 minutes: {result} bugs");
        Assert.Equal(99, result);
    }
    
    [Fact]
    public void TestPart2()
    {
        var sut = new AoC201924();
        var result = sut.Part2();
        output.WriteLine($"After 200 minutes: {result} bugs");
        Assert.True(result > 0);
    }
}
