
namespace AdventOfCode.Year2019.Day25;

public class AoC201925(string[] input)
{
    readonly long[] program = input[0].Split(',').Select(long.Parse).ToArray();

    public AoC201925() : this(Read.InputLines())
    {
    }
    
    public string Part1()
    {
        var solver = new Day25Solver(program);
        return solver.Solve();
    }
    
    public long Part2()
    {
        // Day 25 Part 2 is just a congratulations message - no actual puzzle
        return 0;
    }
}

enum Direction { North, East, South, West }

static class DirectionExtensions
{
    public static string ToCommand(this Direction dir) => dir switch
    {
        Direction.North => "north",
        Direction.East => "east",
        Direction.South => "south",
        Direction.West => "west",
        _ => throw new ArgumentException()
    };
    
    public static Direction Opposite(this Direction dir) => dir switch
    {
        Direction.North => Direction.South,
        Direction.South => Direction.North,
        Direction.East => Direction.West,
        Direction.West => Direction.East,
        _ => throw new ArgumentException()
    };
    
    public static Direction Parse(string s) => s.ToLower() switch
    {
        "north" => Direction.North,
        "east" => Direction.East,
        "south" => Direction.South,
        "west" => Direction.West,
        _ => throw new ArgumentException($"Unknown direction: {s}")
    };
}

readonly record struct ParsedOutput(
    string? RoomName,
    List<string> Doors,
    List<string> Items
);


static class InteractiveIntCode
{
    public static List<string> RunCommands(long[] program, List<string> commands)
    {
        var input = new List<long>();
        foreach (var command in commands)
        {
            foreach (var c in command)
                input.Add(c);
            input.Add(10); // newline
        }
        
        var computer = new IntCode(program);
        var output = new List<long>();
        
        try
        {
            output.AddRange(computer.Run(input));
        }
        catch (Exception ex) when (ex.Message == "No more input")
        {
            // Expected - the game always asks for more input
        }
        
        return ConvertOutputToLines(output);
    }
    
    private static List<string> ConvertOutputToLines(List<long> output)
    {
        var lines = new List<string>();
        var currentLine = new StringBuilder();
        
        foreach (var value in output)
        {
            var c = (char)value;
            if (c == '\n')
            {
                lines.Add(currentLine.ToString());
                currentLine.Clear();
            }
            else
            {
                currentLine.Append(c);
            }
        }
        
        if (currentLine.Length > 0)
            lines.Add(currentLine.ToString());
        
        return lines;
    }
}

partial class Day25Solver
{
    private readonly long[] program;
    
    // Known dangerous items that cause instant game over
    private static readonly HashSet<string> DangerousItems = new()
    {
        "molten lava",
        "infinite loop", 
        "giant electromagnet",
        "photons",
        "escape pod"
    };
    
    public Day25Solver(long[] program)
    {
        this.program = program;
    }
    
    public string Solve()
    {
        // Explore the ship using BFS to find all items and the Security Checkpoint
        var visited = new HashSet<string>();
        var queue = new Queue<List<string>>();
        queue.Enqueue(new List<string>());
        
        var allItems = new List<(string item, List<string> path)>();
        List<string>? pathToCheckpoint = null;
        
        while (queue.Count > 0 && queue.Count < 500) // Safety limit
        {
            var path = queue.Dequeue();
            var pathKey = string.Join("|", path);
            
            if (visited.Contains(pathKey)) continue;
            visited.Add(pathKey);
            
            var output = InteractiveIntCode.RunCommands(program, path);
            var rooms = ParseAllRooms(output);
            
            if (rooms.Count == 0) continue;
            
            var currentRoom = rooms[^1];
            
            // Found the checkpoint - save path but don't enter yet
            if (currentRoom.RoomName == "Security Checkpoint")
            {
                pathToCheckpoint = new List<string>(path);
                continue;
            }
            
            // Don't enter the Pressure Floor yet
            if (currentRoom.RoomName == "Pressure-Sensitive Floor")
                continue;
            
            // Collect safe items
            foreach (var item in currentRoom.Items)
            {
                if (!DangerousItems.Contains(item))
                {
                    allItems.Add((item, new List<string>(path)));
                    var withItem = new List<string>(path) { $"take {item}" };
                    queue.Enqueue(withItem);
                }
            }
            
            // Explore all doors
            foreach (var door in currentRoom.Doors)
            {
                var withMove = new List<string>(path) { door };
                queue.Enqueue(withMove);
            }
        }
        
        if (pathToCheckpoint == null)
            throw new Exception("Security Checkpoint not found");
        
        // Build commands: collect all items and go to checkpoint
        var items = allItems.Select(x => x.item).Distinct().ToList();
        var commands = new List<string>();
        
        foreach (var (item, itemPath) in allItems.GroupBy(x => x.item).Select(g => g.First()))
        {
            commands.AddRange(itemPath);
            commands.Add($"take {item}");
            
            // Return to start
            for (int i = itemPath.Count - 1; i >= 0; i--)
            {
                var dir = DirectionExtensions.Parse(itemPath[i]);
                commands.Add(dir.Opposite().ToCommand());
            }
        }
        
        commands.AddRange(pathToCheckpoint);
        
        // Find which direction leads to Pressure Floor
        var checkpointOutput = InteractiveIntCode.RunCommands(program, pathToCheckpoint);
        var checkpointRoom = ParseAllRooms(checkpointOutput)[^1];
        
        Direction? pressureDirection = null;
        foreach (var doorStr in checkpointRoom.Doors)
        {
            var testPath = new List<string>(pathToCheckpoint) { doorStr };
            var testOutput = InteractiveIntCode.RunCommands(program, testPath);
            if (testOutput.Any(line => line.Contains("Pressure-Sensitive Floor")))
            {
                pressureDirection = DirectionExtensions.Parse(doorStr);
                break;
            }
        }
        
        if (pressureDirection == null)
            throw new Exception("Can't find direction to Pressure Floor");
        
        // Try all item combinations to pass the pressure sensor
        return TryCombinations(commands, items, pressureDirection.Value);
    }
    
    private string TryCombinations(List<string> baseCommands, List<string> items, Direction pressureDirection)
    {
        var combinations = GeneratePowerSet(items).Skip(1); // Skip empty set
        
        foreach (var combination in combinations)
        {
            var commands = new List<string>(baseCommands);
            
            // Drop all items
            foreach (var item in items)
                commands.Add($"drop {item}");
            
            // Take only the items in this combination
            foreach (var item in combination)
                commands.Add($"take {item}");
            
            // Try to pass through to Pressure Floor
            commands.Add(pressureDirection.ToCommand());
            
            var output = InteractiveIntCode.RunCommands(program, commands);
            
            // Check if we got the password
            foreach (var line in output)
            {
                var match = MyRegex().Match(line);
                if (match.Success)
                    return match.Groups[1].Value;
            }
        }
        
        throw new Exception("No combination worked");
    }
    
    private List<ParsedOutput> ParseAllRooms(List<string> output)
    {
        var results = new List<ParsedOutput>();
        var currentSegment = new List<string>();
        
        foreach (var line in output)
        {
            currentSegment.Add(line);
            
            if (line == "Command?")
            {
                var parsed = ParseOutput(currentSegment);
                if (parsed.RoomName != null)
                    results.Add(parsed);
                currentSegment.Clear();
            }
        }
        
        if (currentSegment.Count > 0)
        {
            var parsed = ParseOutput(currentSegment);
            if (parsed.RoomName != null)
                results.Add(parsed);
        }
        
        return results;
    }
    
    private ParsedOutput ParseOutput(List<string> lines)
    {
        string? roomName = null;
        var doors = new List<string>();
        var items = new List<string>();
        
        var isDoorSection = false;
        var isItemSection = false;
        
        foreach (var line in lines)
        {
            if (line.StartsWith("==") && line.EndsWith("=="))
            {
                roomName = line.Trim('=', ' ');
                isDoorSection = false;
                isItemSection = false;
            }
            else if (line == "Doors here lead:")
            {
                isDoorSection = true;
                isItemSection = false;
            }
            else if (line == "Items here:")
            {
                isItemSection = true;
                isDoorSection = false;
            }
            else if (line.StartsWith("- "))
            {
                var item = line[2..];
                if (isDoorSection)
                    doors.Add(item);
                else if (isItemSection)
                    items.Add(item);
            }
            else if (line == "Command?" || line == "")
            {
                isDoorSection = false;
                isItemSection = false;
            }
        }
        
        return new ParsedOutput(roomName, doors, items);
    }
    
    private static IEnumerable<List<T>> GeneratePowerSet<T>(List<T> items)
    {
        int n = items.Count;
        int total = 1 << n;
        
        for (int i = 0; i < total; i++)
        {
            var subset = new List<T>();
            for (int j = 0; j < n; j++)
            {
                if ((i & (1 << j)) != 0)
                    subset.Add(items[j]);
            }
            yield return subset;
        }
    }

    [GeneratedRegex(@"typing (\d+) on the keypad")]
    private static partial Regex MyRegex();
}

public class AoC201925Tests(ITestOutputHelper output)
{
    [Fact]
    public void Part1()
    {
        var aoc = new AoC201925();
        var result = aoc.Part1();
        Assert.NotEmpty(result);
    }
    
    // Discovers all items in the game world
    [Fact()]
    public void DiscoverAllItems()
    {
        var input = Read.SampleLines();
        var program = input[0].Split(',').Select(long.Parse).ToArray();
        
        var visited = new HashSet<string>();
        var queue = new Queue<List<string>>();
        queue.Enqueue(new List<string>());
        
        var allItems = new HashSet<string>();
        var itemLocations = new Dictionary<string, string>();
        
        while (queue.Count > 0 && queue.Count < 500)
        {
            var path = queue.Dequeue();
            var pathKey = string.Join("|", path);
            
            if (visited.Contains(pathKey)) continue;
            visited.Add(pathKey);
            
            var output = InteractiveIntCode.RunCommands(program, path);
            var rooms = ParseRooms(output);
            
            if (rooms.Count == 0) continue;
            
            var currentRoom = rooms[^1];
            
            // Collect all items found
            foreach (var item in currentRoom.Items)
            {
                if (allItems.Add(item))
                {
                    itemLocations[item] = currentRoom.RoomName ?? "Unknown";
                }
            }
            
            // Skip Security Checkpoint and Pressure Floor
            if (currentRoom.RoomName is "Security Checkpoint" or "Pressure-Sensitive Floor")
                continue;
            
            // Explore all doors
            foreach (var door in currentRoom.Doors)
            {
                var withMove = new List<string>(path) { door };
                queue.Enqueue(withMove);
            }
        }
        
        // Output all found items
        output.WriteLine($"Found {allItems.Count} items:");
        foreach (var item in allItems.OrderBy(x => x))
        {
            output.WriteLine($"  - {item} (in {itemLocations[item]})");
        }
    }

    // Helper test to identify dangerous items through experimentation
    [Theory(Skip = "Very slow - only used to validate dangerous items")]
    [InlineData("molten lava", true)]
    [InlineData("infinite loop", true)]
    [InlineData("giant electromagnet", true)]
    [InlineData("photons", true)]
    [InlineData("escape pod", true)]
    [InlineData("food ration", false)]
    [InlineData("mug", false)]
    [InlineData("fuel cell", false)]
    [InlineData("jam", false)]
    [InlineData("loom", false)]
    [InlineData("manifold", false)]
    [InlineData("prime number", false)]
    [InlineData("spool of cat6", false)]
    public void IdentifyDangerousItem(string suspectedItem, bool expected)
    {
        var input = Read.SampleLines();
        var program = input[0].Split(',').Select(long.Parse).ToArray();
        
        // Try to find and take the item
        var visited = new HashSet<string>();
        var queue = new Queue<List<string>>();
        queue.Enqueue(new List<string>());
        
        bool foundAndTook = false;
        
        while (queue.Count > 0 && queue.Count < 200)
        {
            var path = queue.Dequeue();
            var pathKey = string.Join("|", path);
            
            if (visited.Contains(pathKey)) continue;
            visited.Add(pathKey);
            
            var output = InteractiveIntCode.RunCommands(program, path);
            
            // Check if this path led to a problem
            if (output.Any(line => line.Contains("Bye!") || 
                                   line.Contains("eaten by a Grue") ||
                                   line.Contains("stuck") ||
                                   line.Contains("ejected")))
            {
                // This item is dangerous
                foundAndTook = true;
                break;
            }
            
            var rooms = ParseRooms(output);
            if (rooms.Count == 0) continue;
            
            var currentRoom = rooms[^1];
            
            // If we found the item, try to take it
            if (currentRoom.Items.Contains(suspectedItem))
            {
                var withItem = new List<string>(path) { $"take {suspectedItem}" };
                queue.Enqueue(withItem);
                foundAndTook = true;
            }
            
            // Explore
            foreach (var door in currentRoom.Doors)
            {
                var withMove = new List<string>(path) { door };
                queue.Enqueue(withMove);
            }
        }
        
        if (expected != foundAndTook)
        {
            Assert.Fail($"Item '{suspectedItem}' should be found and identified as {(expected?"" :"not")} dangerous");
        }
    }
    
    private static List<ParsedOutput> ParseRooms(List<string> output)
    {
        var results = new List<ParsedOutput>();
        var currentSegment = new List<string>();
        
        foreach (var line in output)
        {
            currentSegment.Add(line);
            
            if (line == "Command?")
            {
                var parsed = ParseRoom(currentSegment);
                if (parsed.RoomName != null)
                    results.Add(parsed);
                currentSegment.Clear();
            }
        }
        
        if (currentSegment.Count > 0)
        {
            var parsed = ParseRoom(currentSegment);
            if (parsed.RoomName != null)
                results.Add(parsed);
        }
        
        return results;
    }
    
    private static ParsedOutput ParseRoom(List<string> lines)
    {
        string? roomName = null;
        var doors = new List<string>();
        var items = new List<string>();
        
        var isDoorSection = false;
        var isItemSection = false;
        
        foreach (var line in lines)
        {
            if (line.StartsWith("==") && line.EndsWith("=="))
            {
                roomName = line.Trim('=', ' ');
                isDoorSection = false;
                isItemSection = false;
            }
            else if (line == "Doors here lead:")
            {
                isDoorSection = true;
                isItemSection = false;
            }
            else if (line == "Items here:")
            {
                isItemSection = true;
                isDoorSection = false;
            }
            else if (line.StartsWith("- "))
            {
                var item = line[2..];
                if (isDoorSection)
                    doors.Add(item);
                else if (isItemSection)
                    items.Add(item);
            }
            else if (line == "Command?" || line == "")
            {
                isDoorSection = false;
                isItemSection = false;
            }
        }
        
        return new ParsedOutput(roomName, doors, items);
    }
}