using System.Collections;

namespace AdventOfCode.Year2021.Day16;

public class AoC202116
{
    static string input = Read.InputLines()[0];
    public object Part1() => new Packet(input.ToBinary()).GetVersionSum();

    public object Part2() => new Packet(input.ToBinary()).Value;
}

static class Ex
{
    public static string ToBinary(this string input) => string.Join(string.Empty, input.Select(c => Convert.ToString(
        Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0'))
        );
}

internal class Packet
{
    private int Length;
    public IEnumerable<Packet> Children => children;
    private List<Packet> children { get; } = new();
    private int version;
    private int type;
    private string binary;
    public Packet(string binary)
    {
        this.binary = binary;

        // header
        version = Convert.ToInt32(Read(3).ToString(), 2);
        type = Convert.ToInt32(Read(3).ToString(), 2);

        // data
        if (type == 4)
        {
            StringBuilder result = new();
            bool readLastGroup = false;
            while (!readLastGroup)
            {
                if (Read(1)[0] == '0')
                    readLastGroup = true;
                result.Append(Read(4));
            }
            Value = Convert.ToInt64(result.ToString(), 2);
        }
        else
        {

            var subpackets = (Read(1)[0] switch
            {
                '0' => ReadNumberOfBits(Convert.ToInt32(Read(15).ToString(), 2)),
                _ => ReadNumberOfPackets(Convert.ToInt32(Read(11).ToString(), 2))
            }).ToList();

            children.AddRange(subpackets);
            var subValues = subpackets.Select(packet => packet.Value);
            Value = type switch
            {
                0 => subValues.Sum(),
                1 => subValues.Aggregate((long)1, (x, y) => x * y),
                2 => subValues.Min(),
                3 => subValues.Max(),
                4 => Value,
                5 => subValues.First() > subValues.Last() ? 1 : 0,
                6 => subValues.First() < subValues.Last() ? 1 : 0,
                7 => subValues.First() == subValues.Last() ? 1 : 0,
                _ => throw new NotImplementedException(),
            };
        }
    }
    public long Value { get; set; }
    public int GetVersionSum() => children.Aggregate(version, (s, sub) => s + sub.GetVersionSum());

    private ReadOnlySpan<char> Read(int amount)
    {
        var span = binary.AsSpan(Length, amount);
        Length += amount;
        return span;
    }

    private IEnumerable<Packet> ReadNumberOfPackets(int number)
    {
        var str = binary.Substring(Length);
        for (int i = 0; i < number; i++)
        {
            var subpacket = new Packet(str);
            yield return subpacket;
            str = str.Substring(subpacket.Length);
            Length += subpacket.Length;
        }
    }

    private IEnumerable<Packet> ReadNumberOfBits(int length)
    {
        string str = binary.Substring(Length);
        int bytesRead = 0;
        while (bytesRead < length)
        {
            Packet subpacket = new Packet(str);
            yield return subpacket;
            str = str.Substring(subpacket.Length);
            Length += subpacket.Length;
            bytesRead += subpacket.Length;
        }
    }
}
