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
    public long Value { get; set; }

    private int Length;
    private List<Packet> Children { get; } = new();
    private int version;
    private int type;
    private string binary;

    public Packet(string binary)
    {
        this.binary = binary;

        // header
        version = Convert.ToInt32(ReadSpan(3).ToString(), 2);
        type = Convert.ToInt32(ReadSpan(3).ToString(), 2);

        // data
        if (type == 4)
        {
            Value = ReadValue();
        }
        else
        {
            var subpackets = (ReadSpan(1)[0] switch
            {
                '0' => ReadNumberOfBits(Convert.ToInt32(ReadSpan(15).ToString(), 2)),
                _ => ReadNumberOfPackets(Convert.ToInt32(ReadSpan(11).ToString(), 2))
            }).ToList();

            Children.AddRange(subpackets);
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

    private long ReadValue()
    {
        StringBuilder result = new();
        bool done = false;
        while (!done)
        {
            if (ReadSpan(1)[0] == '0')
                done = true;
            result.Append(ReadSpan(4));
        }
        return Convert.ToInt64(result.ToString(), 2);
    }

    public int GetVersionSum() => Children.Aggregate(version, (s, sub) => s + sub.GetVersionSum());

    private ReadOnlySpan<char> ReadSpan(int amount)
    {
        var span = binary.AsSpan(Length, amount);
        Length += amount;
        return span;
    }

    Packet ReadOnePacket()
    {
        var subpacket = new Packet(binary.Substring(Length));
        Length += subpacket.Length;
        return subpacket;
    }

    private IEnumerable<Packet> ReadNumberOfPackets(int number)
    {
        for (int i = 0; i < number; i++)
        {
            yield return ReadOnePacket();
        }
    }

    private IEnumerable<Packet> ReadNumberOfBits(int length)
    {
        var max = Length + length;
        while (Length < max)
        {
            yield return ReadOnePacket();
        }
    }
}
