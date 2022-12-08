using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace AdventOfCode.Year2016.Day14;

public class AoC201614
{
    //readonly string salt = "abc";
    readonly string salt = "zpqevtbw";
    string CurrentPath([CallerFilePath] string path = "") => Path.GetDirectoryName(path) ?? "";
    public object Part1()
    {
        Dictionary<int, string> hashes = new();
        var q = (
            from i in Range(1, int.MaxValue)
            let repeat = Find3(hashes, salt, i)
            where repeat.HasValue
            from j in Range(i + 1, 1000).SkipWhile(j => !IsValid2(hashes, salt, j, repeat.Value)).Take(1)
            select i
            ).Take(64);

        {
            int n = 1;
            int index = 0;
            foreach (var item in q)
            {
                Console.WriteLine($"{n++}: {item}");
                index = item;
            }
            return index;
        }
    }

    private int Write(int i,  char c, int j)
    {
        Console.WriteLine($"{i} - {c} - {j}");
        return i;
    }

    static MD5 md5 = MD5.Create();

    static string GetHash(Dictionary<int, string> hashes, string salt, int seed, int repeat = 0)
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
            var hashStr = ToString(hash).ToLower();
            char? three = null;
            for (int i = 0; i < hashStr.Length - 3; i++)
            {
                if (hashStr[i] == hashStr[i + 1] && hashStr[i] == hashStr[i+2])
                {
                    three = hashStr[i];
                    //Console.WriteLine($"{hashStr} has {three}{three}{three} at {i}");
                    break;
                }
            }
            var fives = new List<char>();
            for (int i = 0; i < hashStr.Length - 5; i++)
            {
                if (hashStr[i] == hashStr[i + 1] && hashStr[i] == hashStr[i + 2] && hashStr[i] == hashStr[i+3] && hashStr[i] == hashStr[i+4])
                {
                    fives.Add(hashStr[i]);
                    var c = hashStr[i];
                    Console.WriteLine($"{hashStr} has {c}{c}{c}{c}{c} at {i}");
                    break;
                }
            }
            hashes[seed] = hashStr;
        }
        return hashes[seed];
    }

    static char? Find3(Dictionary<int, string> hashes, string salt, int seed, int repeat = 0)
    {
        var hash = GetHash(hashes, salt, seed, repeat).AsSpan();
        for (int i = 0; i < hash.Length - 3; i++)
        {
            var c = hash[i];
            if (hash[i + 1] == c && hash[i + 2] == c) return c;
        }
        return null;

    }
    static bool IsValid2(Dictionary<int, string> hashes, string salt, int seed, char c, int repeat = 0)
    {
        var hash = GetHash(hashes, salt, seed, repeat);
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
        Dictionary<int, string> hashes = new();
        var path = Path.Combine(CurrentPath(), "hashes-extended.txt");
        if (!File.Exists(path))
        {
            using var s = new StreamWriter(File.OpenWrite(path));
            foreach (var i in Range(0, 50000))
            {
                var hash = GetHash(hashes, salt, i, 2016);
                s.WriteLine(hash);
            }
        }

        var q = (
            from i in Range(1, int.MaxValue)
            let repeat = Find3(hashes, salt, i, 2016)
            where repeat.HasValue
            from j in Range(i + 1, 1000).SkipWhile(j => !IsValid2(hashes, salt, j, repeat.Value, 2016)).Take(1)
            select i
            ).Take(64);
        return q.Last();
    }
}