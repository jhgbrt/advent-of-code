namespace AdventOfCode.Year2021.Day08;

public class AoC202108 : AoCBase
{
    static string[] input = Read.SampleLines(typeof(AoC202108));
    public override object Part1() => (from p in input
                                       from value in p.Split('|').Last().Split(' ')
                                       where value.Length is 2 or 3 or 4 or 7
                                       select value
                                       ).Count();
    public override object Part2() => (
        from p in input
        let fragments = p.Split('|')
        let input = fragments[0].Split(' ')
        let output = fragments[1].Split(' ')
        select Decode(input, output)
        ).Sum();

    private long Decode(string[] input, string[] output)
    {

        var A = 0b1000000; // <=  
        var B = 0b0100000;
        var C = 0b0010000;
        var D = 0b0001000;
        var E = 0b0000100;
        var F = 0b0000010;
        var G = 0b0000001;

        var _1 = 0b0110000; Debug.Assert(_1 == (B & C));// n = 2  B & C
        var _4 = 0b0110011; Debug.Assert(_4 == (B & C & F & G)); // n = 4  
        var _8 = 0b1111111; // n = 8
        var _7 = 0b1110000; // n = 3
        var _2 = 0b1101101; // n = 5
        var _3 = 0b1111001;
        var _5 = 0b1011011;
        var _9 = 0b1111011; // n = 6
        var _6 = 0b1011111;

        return 0;
    }

    /*
           A
         F     B
            G
         E     C
            D

        ABCDEFG N    
    1 0b0110000 2   
    4 0b0110011 4   
    8 0b1111111 8
    7 0b1110000 3

    2 0b1101101 5   
    3 0b1111001 5   
    5 0b1011011 5

    9 0b1111011 6
    6 0b1011111 6

     */


    IDictionary<string, int> Decoder(string[] input)
    {
        return null;

        var mapping = new[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g' }.ToDictionary(c => c, c => "abcdefg");


        while (mapping.Values.Any(s => s.Length > 1))
        {

        }



        foreach (var i in input)
        {
            var digit = i.Length switch
            {
                2 => new[] { 1 },
                4 => new[] { 4 },
                3 => new[] { 7 },
                8 => new[] { 8 },
                6 => new[] { 6, 9 },
                5 => new[] { 2, 3, 5 }
            };

        }

    }
}

