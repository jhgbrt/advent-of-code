using System.Security.Cryptography;

namespace AdventOfCode.Year2016.Day14;

public class AoC201614
{
    string salt = "zpqevtbw";
    public object Part1()
    {
        var q = (
            from i in Range(1, int.MaxValue)
            let repeat = Find3(salt, i)
            where repeat.HasValue
            from j in Range(i + 1, 1000).SkipWhile(j => !IsValid2(salt, j, repeat.Value)).Take(1)
            select i
            ).Take(64);
        return q.Last();
    }
    static MD5 md5 = MD5.Create();
    static Dictionary<int, string> hashes = new();
    static string GetHash(string salt, int seed, int repeat = 0)
    {
        if (!hashes.ContainsKey(seed))
        {
            var key = $"{salt}{seed}";
            var hash = md5.ComputeHash(Encoding.ASCII.GetBytes(key));
            for (int i = 0; i < repeat; i++)
            {
                var bytes = Encoding.ASCII.GetBytes(ToString(hash).ToLower());
                hash = md5.ComputeHash(bytes);
            }
            hashes[seed] = ToString(hash).ToLower();
        }
        return hashes[seed];
    }

    static char? Find3(string salt, int seed, int repeat = 0)
    {
        var hash = GetHash(salt, seed, repeat).AsSpan();
        for (int i = 0; i < hash.Length - 3; i++)
        {
            var c = hash[i];
            if (hash[i + 1] == c && hash[i + 2] == c) return c;
        }
        return null;

    }
    static bool IsValid2(string salt, int seed, char c, int repeat = 0)
    {
        var hash = GetHash(salt, seed, repeat);
        for (int i = 0; i < hash.Length - 5; i++)
        {
            if (hash[i] == c
                && hash[i + 1] == c
                && hash[i + 2] == c
                && hash[i + 3] == c
                && hash[i + 4] == c
                ) return true;
        }
        return false;
    }

    static string ToString(byte[] hash)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < hash.Length; i++)
        {
            sb.Append(hash[i].ToString("X2"));
        }
        return sb.ToString();
    }

    public object Part2()
    {
        hashes.Clear();
        var q = (
            from i in Range(1, int.MaxValue)
            let repeat = Find3(salt, i, 2016)
            where repeat.HasValue
            from j in Range(i + 1, 1000).SkipWhile(j => !IsValid2(salt, j, repeat.Value, 2016)).Take(1)
            select i
            ).Take(64);
        return q.Last();
    }
}