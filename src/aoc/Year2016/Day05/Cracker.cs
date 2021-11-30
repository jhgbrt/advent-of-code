using System.Security.Cryptography;

namespace AdventOfCode.Year2016.Day05;

class Cracker
{
    MD5 _md5 = MD5.Create();
    public bool StartsWith5Zeroes(byte[] hash) => hash[0] == 0 && hash[1] == 0 && (hash[2] & 0x0F) == hash[2];

    internal string GeneratePassword1(string input, int length)
    {
        StringBuilder sb = new StringBuilder();
        var doorid = input;
        int i = 0;
        while (true)
        {
            var s = $"{doorid}{i}";
            var hash = _md5.ComputeHash(Encoding.ASCII.GetBytes(s));
            if (StartsWith5Zeroes(hash))
            {
                var str = BitConverter.ToString(hash).Replace("-", "");
                sb.Append(str[5]);
                Console.Write(str[5]);
            }
            if (sb.Length == length)
                break;
            i++;
        }
        Console.WriteLine();
        var password = sb.ToString();
        return password.ToLower();
    }
    internal string GeneratePassword2(string input, int length)
    {
        var sb = Enumerable.Repeat('_', length).ToArray();
        var doorid = input;
        int i = 0;
        while (true)
        {
            var s = $"{doorid}{i}";
            var hash = _md5.ComputeHash(Encoding.ASCII.GetBytes(s));
            if (StartsWith5Zeroes(hash))
            {
                var str = BitConverter.ToString(hash).Replace("-", "");
                if (
                    int.TryParse(str.Substring(5, 1), out int position)
                    && position >= 0 && position < 8 && sb[position] == '_'
                    )
                {
                    sb[position] = str[6];
                    Console.WriteLine(new String(sb));
                }
            }
            if (sb.All(c => c != '_'))
                break;
            i++;
        }
        var password = new string(sb);
        return password.ToLower();
    }
}