namespace AdventOfCode.Year2017.Day14;

public class AoC201714
{
    static string key = "hxtvlmkl";
    public object Part1() => Defrag.CountBitsInGrid(key);
    public object Part2() => Defrag.CountRegions(key);

}

static class Defrag
{
    internal static int CountBits(uint value)
    {
        var count = 0;
        while (value != 0)
        {
            count++;
            value &= value - 1;
        }
        return count;
    }

    static readonly int[] offsets = { 0, 8, 16, 24 };
    private static IEnumerable<int> _0_to_127 = Range(0, 128).ToArray();

    public static int CountBitsInGrid(string key)
    {
        return (
            from i in _0_to_127
            let hash = KnotsHash.Hash($"{key}-{i}")
            from offset in offsets
            select CountBits(Convert.ToUInt32($"0x{hash.Substring(offset, 8)}", 16))
        ).Sum();
    }

    public static int CountRegions(string key)
    {
        var grid = new bool[128, 128];

        var rows = _0_to_127.Select(i => (i: i, row: KnotsHash.Hash($"{key}-{i}").ToBinary()));

        foreach ((int row, string hash) in rows)
            foreach (var col in _0_to_127)
                grid[row, col] = hash[col] == '1';

        var positions =
            from pos in AllPositions(128, 128)
            where grid[pos.x, pos.y]
            select pos;

        var count = 0;
        foreach (var (x, y) in positions)
        {
            ClearGroup(x, y, grid);
            count++;
        }

        return count;
    }

    private static void ClearGroup(int x, int y, bool[,] diskBits)
    {
        diskBits[x, y] = false;

        var toclear =
            from t in GetNeighbors(x, y)
            where t.x >= 0 && t.x < 128 && t.y >= 0 && t.y < 128 && diskBits[t.x, t.y]
            select t;

        foreach (var t in toclear)
        {
            ClearGroup(t.x, t.y, diskBits);
        }
    }

    public static IEnumerable<(int x, int y)> GetNeighbors(int x, int y)
    {
        yield return (x - 1, y);
        yield return (x + 1, y);
        yield return (x, y - 1);
        yield return (x, y + 1);
    }

    static IEnumerable<(int x, int y)> AllPositions(int max1, int max2)
    {
        for (var x = 0; x < 128; x++)
            for (var y = 0; y < 128; y++)
            {
                yield return (x, y);
            }

    }
}


static class Ex
{
    public static string ToBinary(this string hex)
    {
        var sb = new StringBuilder();

        foreach (var c in hex.ToCharArray())
        {
            var v = int.Parse(c.ToString(), NumberStyles.HexNumber);
            sb.Append(Convert.ToString(v, 2).PadLeft(4, '0'));
        }

        return sb.ToString();
    }
}


static class KnotsHash
{
    public static string Hash(string input)
    {
        var bytes = Encoding.ASCII.GetBytes(input).Concat(new byte[] { 17, 31, 73, 47, 23 }).ToArray();
        var array = Hash(bytes, 256, 64).ReduceHash().ToArray();
        var hex = BitConverter.ToString(array).Replace("-", "").ToLower();
        return hex;
    }

    internal static byte[] Hash(byte[] input, int length = 256, int rounds = 1)
    {
        var result = Enumerable.Range(0, length).Select(i => (byte)i).ToArray();
        var skip = 0;
        var index = 0;
        for (var i = 0; i < rounds; i++)
        {
            foreach (var l in input)
            {
                Step(result, index, l);
                index += l + skip;
                skip++;
            }
        }
        return result;
    }

    internal static void Step(byte[] result, int index, int l)
    {
        var slice = result.CircularSlice(index, l).Reverse().ToList();
        for (int i = 0; i < l; i++)
        {
            result[(index + i) % result.Length] = slice[i];
        }
    }

    internal static IEnumerable<T> CircularSlice<T>(this T[] input, int index, int length)
    {
        for (int i = 0; i < length; i++)
            yield return input[(index + i) % input.Length];
    }

    internal static IEnumerable<byte> ReduceHash(this byte[] input)
    {
        for (int i = 0; i < input.Length; i += 16)
        {
            yield return input.Skip(i).Take(16).Aggregate((l, r) => (byte)(l ^ r));
        }
    }

}
