using System.Collections;

namespace AdventOfCode.Year2021.Day16;

public class AoC202116
{
    static string input = Read.SampleText();

    public object Part1()
    {
        Console.WriteLine(input);
        var binary = input.ToBinary();

        Console.WriteLine(binary);

        var packets = binary.DecodePackets();

        return 0;
    }

    public object Part2() => -1;






}

static class Decoder
{
    public static string ToBinary(this string input) => string.Join(string.Empty, input.Select(c => Convert.ToString(
        Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0'))
        );

    public static IEnumerable<Packet> DecodePackets(this string binary)
    {
        var index = 0;

        while (index < binary.Length)
        {
            (var v, var id, var delta) = GetHeader(binary, index..6);
            Console.WriteLine((v, id, delta));
            index += 6;

            switch (id)
            {
                case 4:
                    var literal = Literal(binary, id, v, index..);
                    yield return literal;
                    break;
                case 6:
                    switch (binary[index])
                    {
                        case '1':
                            {
                                index++;
                                var length = 11;
                                var n = Decode(binary, index..(index + length));
                                Console.WriteLine(n);
                            }
                            break;
                        case '0':
                            {
                                index++;
                                var length = 15;
                                var subpacketsLength = Decode(binary, index..(index + length));
                                index += length;
                                Assert.Equal(27, subpacketsLength);
                                var subpackets = binary[index..(index+ subpacketsLength)];
                                Assert.Equal("110100010100101001000100100", subpackets);
                                foreach (var packet in subpackets.DecodePackets())
                                    yield return packet;
                                Console.WriteLine(subpacketsLength);
                            }
                            break;
                        default:
                            throw new Exception();
                    }

                    break;

            }


            break;
        }
    }



    static (int v, int d, int l) GetHeader(string s, Range r)
    {
        var v = Convert.ToInt32(s[r][0..3], 2);
        var id = Convert.ToInt32(s[r][3..6], 2);
        return (v, id, 6);
    }

    static int Decode(string s, Range r) => Convert.ToInt32(s[r], 2);
    public static Literal Literal(this string s, int id, int version, Range r)
    {
        return new Literal(
            id,
            version,
            Convert.ToInt32(s[r][1..5] + s[r][6..10] + s[r][11..15], 2)
            );
    }
}

interface Packet { }
record Literal(int id, int version, int value) : Packet;

public class Tests
{
    [Fact]
    public void Test1()
    {
        var literal = "D2FE28";
        var binary = literal.ToBinary();
        var decoded = binary.DecodePackets().Single();
        Assert.True(decoded is Literal);
        var l = (Literal)decoded;
        Assert.Equal(4, l.id);
        Assert.Equal(6, l.version);
        Assert.Equal(2021, l.value);
    }

    [Fact]
    public void Test2()
    {
        var hex = "38006F45291200";
        var binary = hex.ToBinary();
        var decoded = binary.DecodePackets().ToList();
        var count = decoded.Count();
        //Assert.Equal(2, count);
    }

}