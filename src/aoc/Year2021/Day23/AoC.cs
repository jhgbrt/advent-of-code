namespace AdventOfCode.Year2021.Day23;

public class AoC202123
{
    static string[] input = Read.InputLines();
    public object Part1()
    {
        var energy = new[] { 1, 10, 100, 1000 };

        var rooms = new Dictionary<int, char>
        {
            [0] = 'B',
            [1] = 'A',
            [2] = 'C',
            [3] = 'D',
            [4] = 'B',
            [5] = 'C',
            [6] = 'D',
            [7] = 'A'
        };
        var hallways = Range(0, 11).ToArray();

        foreach (var room in rooms.Keys)
            foreach (var hallway in hallways)
            {
                var steps = Steps(room, hallway);
                Console.WriteLine($"{(r: room, h: hallway, c: steps)}");
            }
        return 0;

    }

    private static int Steps(int r, int h)
    {
        // #############       
        // #01234567890#       
        // ###0#2#4#6###
        //   #1#3#5#7#
        //   #########

        var row = 1 + r % 2;
        var col = (r % 2) == 0 ? r + 2 : r + 1;
        return row + Math.Abs(h - col);
    }

    
    public object Part2() => -1;
}



