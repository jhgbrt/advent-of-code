using System.Collections;

namespace AdventOfCode.Year2021.Day16;

public class AoC202116
{
    static string input = Read.InputLines()[0];
    public object Part1()
    {
        var binary = input.ToBinary();
        Packet2 packet = new Packet2(binary);
        return GetVersionSum(packet);


        //var packets = binary.GetEnumerator().DecodePackets();

        //return 0;
    }
    private static int GetVersionSum(Packet2 packet)
    {
        int sum = packet.version;
        foreach (Packet2 subpacket in packet.subpackets)
        {
            sum += GetVersionSum(subpacket);
        }
        return sum;
    }
    public object Part2()
    {
        var binary = input.ToBinary();
        Packet2 packet = new Packet2(binary);
        return packet.value;
    }
}

static class Decoder
{
    public static string ToBinary(this string input) => string.Join(string.Empty, input.Select(c => Convert.ToString(
        Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0'))
        );

    static int Read(IEnumerator<char> enumerator, StringBuilder sb, int n)
    {
        sb.Clear();
        int i = 0;
        while (i < n && enumerator.MoveNext())
        {
            sb.Append(enumerator.Current);
            i++;
        }
        return i;
    }

    static int ReadLiteral(IEnumerator<char> enumerator, StringBuilder sb)
    {
        sb.Clear();
        StringBuilder inner = new StringBuilder();
        bool done = false;
        int read = 0;
        while (!done)
        {
            read += Read(enumerator, inner, 5);
            done = inner[0] == '0';
            sb.Append(inner.Remove(0, 1));
        }
        return read;
    }

    static int ReadSubPacketBits(IEnumerator<char> enumerator, StringBuilder sb)
    {
        return 0;
    }
    static IEnumerable<IPacket> ReadSubPackets(IEnumerator<char> enumerator, StringBuilder sb, int n)
    {
        int i = 0;
        while (i < n)
        {

            i++;
        }
        yield break;
    }

    static int ReadInt(IEnumerator<char> enumerator, int n)
    {
        var sb = new StringBuilder();
        Read(enumerator, sb, n);
        return Convert.ToInt32(sb.ToString(), 2);
    }

    public static IEnumerable<IPacket> DecodePackets(this IEnumerator<char> binary)
    {
        var sb = new StringBuilder();

        for (int x = 0; x<100; x++)
        {
            int read = 0;
            
            var v = ReadInt(binary, 3);
            var id = ReadInt(binary, 3);

            switch (id)
            {
                case 4:
                    read = ReadLiteral(binary, sb);
                    if (read == 0) yield break;
                    var literal = new Literal(id, v, Convert.ToInt32(sb.ToString(), 2));
                    yield return literal;
                    break;
                case 6:
                    Read(binary, sb, 1);
                    switch (sb[0])
                    {
                        case '1':
                            {
                                Read(binary, sb, 11);
                                var n = Convert.ToInt32(sb.ToString(), 2);
                                //yield return new Packet { SubPackets = ReadSubPackets(binary, n) };
                            }
                            break;
                        case '0':
                            {
                                Read(binary, sb, 15);
                                var n = Convert.ToInt32(sb.ToString(), 2);

                                //int bytesRead = 0;
                                //while (bytesRead < lengthOfSubpackets)
                                //{
                                //    Packet2 subpacket = new Packet2(bitStream);
                                //    subpackets.Add(subpacket);

                                //    bitStream = bitStream.Skip(subpacket.bytesRead);
                                //    bytesRead += subpacket.bytesRead;
                                //}
                                //return bytesRead;
                                
                                //index++;
                                //var length = 15;
                                //var subpacketsLength = Convert.ToInt16(binary[index..(index + length)], 2);
                                //index += length;
                                //Assert.Equal(27, subpacketsLength);
                                //var subpackets = binary[index..(index+ subpacketsLength)];
                                //Assert.Equal("110100010100101001000100100", subpackets);
                                //yield return new Packet { SubPackets = subpackets.DecodePackets().ToList() };
                                //index += subpacketsLength;
                            }
                            break;
                        default:
                            throw new Exception();
                    }

                    break;

            }

        }
    }

    public static Literal Literal(this string s, int id, int version, Range r)
    {
        return new Literal(
            id,
            version,
            Convert.ToInt32(s[r][1..5] + s[r][6..10] + s[r][11..15], 2)
            );
    }
}
interface IPacket { }
class Packet : IPacket
{
    public List<IPacket>? SubPackets { get; set; }
}

record Literal(int id, int version, int value) : IPacket;

public class Tests
{
    [Fact]
    public void Test1()
    {
        //var literal = "D2FE28";
        //var p = new Packet2(literal.ToBinary());
        //var binary = literal.ToBinary();
        //var decoded = binary.GetEnumerator().DecodePackets().Single();
        //Assert.True(decoded is Literal);
        //var l = (Literal)decoded;
        //Assert.Equal(4, l.id);
        //Assert.Equal(6, l.version);
        //Assert.Equal(2021, l.value);
    }

    [Fact]
    public void Test2()
    {
        //var hex = "38006F45291200";
        //var p = new Packet2(hex.ToBinary());
        //var binary = hex.ToBinary();
        //var decoded = binary.GetEnumerator().DecodePackets().ToList();
        //var count = decoded.Count();
        //Assert.Equal(1, count);
        //var pa = decoded.Single();
        //Assert.Equal(2, ((Packet)pa).SubPackets.Count);
    }

}


internal class Packet2
{
    public int bytesRead;
    public List<Packet2> subpackets;
    public int version;
    public TypeID typeID;
    public long value = long.MinValue;
    public IEnumerable<char> bitStream;

    public Packet2(IEnumerable<char> _bitStream)
    {
        this.bitStream = _bitStream;
        subpackets = new();

        // Read header
        version = Convert.ToInt32(new string(ReadStream(3).ToArray()), 2);
        typeID = (TypeID)Convert.ToInt32(new string(ReadStream(3).ToArray()), 2);

        // Parse data
        if (typeID == TypeID.LITERAL)
        {
            StringBuilder result = new();
            bool readLastGroup = false;
            while (!readLastGroup)
            {
                if (ReadStream(1).First() == '0')
                    readLastGroup = true;
                result.Append(ReadStream(4).ToArray());
            }
            value = Convert.ToInt64(result.ToString(), 2);
        }
        else
        {
            if (ReadStream(1).First() == '0') // Extract subpackets by length
            {
                int lengthOfSubpackets = Convert.ToInt32(new string(ReadStream(15).ToArray()), 2);
                bytesRead += ExtractSubpacketsByLength(lengthOfSubpackets, bitStream);
            }
            else // Extract subpackets by number
            {
                int numberOfSubpackets = Convert.ToInt32(new string(ReadStream(11).ToArray()), 2);
                bytesRead += ExtractSubpacketsByNumber(numberOfSubpackets, bitStream);
            }

            // Get value based on subvalues
            var subValues = subpackets.Select(packet => packet.value);
            switch (typeID)
            {
                case TypeID.SUM:
                    value = subValues.Sum();
                    break;
                case TypeID.PRODUCT:
                    value = subValues.Aggregate((long)1, (x, y) => x * y);
                    break;
                case TypeID.MINIMUM:
                    value = subValues.Min();
                    break;
                case TypeID.MAXIMUM:
                    value = subValues.Max();
                    break;
                case TypeID.GREATER_THAN:
                    value = subValues.First() > subValues.Last() ? 1 : 0;
                    break;
                case TypeID.LESS_THAN:
                    value = subValues.First() < subValues.Last() ? 1 : 0;
                    break;
                case TypeID.EQUALS:
                    value = subValues.First() == subValues.Last() ? 1 : 0;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }

    private IEnumerable<char> ReadStream(int amount)
    {
        IEnumerable<char> result = bitStream.Take(amount);
        bytesRead += amount;
        bitStream = bitStream.Skip(amount);
        return result;
    }

    private int ExtractSubpacketsByNumber(int numberOfSubpackets, IEnumerable<char> bitStream)
    {
        int packetsAdded = 0;
        while (packetsAdded++ < numberOfSubpackets)
        {
            subpackets.Add(new Packet2(bitStream));
            bitStream = bitStream.Skip(subpackets.Last().bytesRead);

        }

        return subpackets.Select(packet => packet.bytesRead).Sum();
    }

    private int ExtractSubpacketsByLength(int lengthOfSubpackets, IEnumerable<char> bitStream)
    {
        int bytesRead = 0;
        while (bytesRead < lengthOfSubpackets)
        {
            Packet2 subpacket = new Packet2(bitStream);
            subpackets.Add(subpacket);

            bitStream = bitStream.Skip(subpacket.bytesRead);
            bytesRead += subpacket.bytesRead;
        }
        return bytesRead;
    }
}

enum TypeID
{
    SUM = 0,
    PRODUCT = 1,
    MINIMUM = 2,
    MAXIMUM = 3,
    LITERAL = 4,
    GREATER_THAN = 5,
    LESS_THAN = 6,
    EQUALS = 7

}