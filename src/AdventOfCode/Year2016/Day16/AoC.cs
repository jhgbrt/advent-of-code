namespace AdventOfCode.Year2016.Day16;

public class AoC201616
{
    string input = Read.InputLines()[0];
    public object Part1()
    {
        var data = GenerateData(input, 272);
        return CalculateChecksum(data);
    }
    public object Part2()
    {
        var data = GenerateData(input, 35651584);
        return CalculateChecksum(data);
    }
    private static string GenerateData(string input, int required)
    {
        var a = input;
        while (a.Length < required)
        {
            var b = new string(a.Reverse().ToArray()).Replace('1', '-').Replace('0', '1').Replace('-', '0');
            a = $"{a}0{b}";
        }
        return a.Substring(0, required);
    }

    private static string CalculateChecksum(string input)
    {
        string checksum = input;
        var sb = new StringBuilder();
        while (checksum.Length % 2 == 0)
        {
            sb.Clear();
            for (var i = 0; i < checksum.Length - 1; i += 2)
            {
                sb.Append(checksum[i] == checksum[i + 1] ? '1' : '0');
            }
            checksum = sb.ToString();
        }
        return checksum;
    }

 
}