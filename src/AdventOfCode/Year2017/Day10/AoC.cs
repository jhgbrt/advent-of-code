namespace AdventOfCode.Year2017.Day10;

public class AoC201710
{
    static string input = "206,63,255,131,65,80,238,157,254,24,133,2,16,0,1,3";
    public object Part1()
    {
        var result = KnotsHash.Hash(input.Split(',').Select(byte.Parse).ToArray());
        var value = result[0] * result[1];
        return value;
    }
    public object Part2() => KnotsHash.Hash(input);
}

static class KnotsHash
{
    internal static string Hash(string input)
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
        for (var i = 0; i < l; i++)
        {
            result[(index + i) % result.Length] = slice[i];
        }
    }

    internal static IEnumerable<T> CircularSlice<T>(this T[] input, int index, int length)
    {
        for (var i = 0; i < length; i++)
            yield return input[(index + i) % input.Length];
    }

    internal static IEnumerable<byte> ReduceHash(this byte[] input)
    {
        for (var i = 0; i < input.Length; i += 16)
        {
            yield return input.Skip(i).Take(16).Aggregate((l, r) => (byte)(l ^ r));
        }
    }

}
