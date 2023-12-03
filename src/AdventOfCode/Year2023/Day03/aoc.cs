using AdventOfCode.Common;

namespace AdventOfCode.Year2023.Day03;
public class AoC202303
{
    static string[] input = Read.InputLines();
    static FiniteGrid grid = new FiniteGrid(input);
    static SerialNr[] serialnrs = FindSerialNrs(grid).ToArray();

    public long Part1() => (from seq in serialnrs
                            let box = grid.BoundingBox(seq.position, seq.Length)
                            where box.Any(c => !char.IsDigit(grid[c]) && grid[c] != '.')
                            select seq.Value).Sum();

    public long Part2() => (from c in grid.Points().Where(c => grid[c] == '*')
                            let adjacentSerialNrs = (
                                from seq in serialnrs
                                let box = grid.BoundingBox(c, 1)
                                where box.Intersect(seq.Coordinates()).Any()
                                select seq.Value
                            ).ToArray()
                            where adjacentSerialNrs.Length == 2
                            select adjacentSerialNrs[0] * adjacentSerialNrs[1]
                            ).Sum();

    static IEnumerable<SerialNr> FindSerialNrs(FiniteGrid grid)
    {
        for (int y = 0; y < grid.Height; y++)
        {
            var x = 0;
            while (x < grid.Width)
            {
                while (!char.IsDigit(grid[x, y]) && x < grid.Width) x++;
                if (x >= grid.Width) continue;
                var position = new Coordinate(x, y);
                var sb = new StringBuilder();
                while (char.IsDigit(grid[x, y]) && x < grid.Width)
                {
                    sb.Append(grid[x, y]);
                    x++;
                }
                yield return new(position, sb.ToString());
            }
        }
    }

    record SerialNr(Coordinate position, string snr) 
    {
        public int Length => snr.Length;
        public long Value => long.Parse(snr);
        public IEnumerable<Coordinate> Coordinates() => Range(position.x, snr.Length).Select(x => new Coordinate(x, position.y));
    }
}
