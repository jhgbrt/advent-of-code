using System.Collections.Generic;
using System.Security.Cryptography;

namespace AdventOfCode.Year2016.Day14;

public class AoC201614
{
    string salt = "abc";
    public object Part1()
    {
        Dictionary<int, (string hash, char? three, List<char> fives)> hashes = new();
        var q = (
            from i in Range(0, int.MaxValue)
            let repeat = Find3(hashes, salt, i)
            where repeat.HasValue
            from j in Range(i + 1, 1000).SkipWhile(j => !IsValid2(hashes, salt, j, repeat.Value)).Take(1)
            select (i, repeat.Value, j)
            ).Take(64);

        {
            int n = 1;
            int index = 0;
            foreach (var item in q)
            {
                Console.WriteLine($"{n++}: {item}");
                index = item.i;
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
    static (string hash, char? three, List<char> fives) GetHash(Dictionary<int, (string hash, char? three, List<char> fives)> hashes, string salt, int seed, int repeat = 0)
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
            hashes[seed] = (hashStr, three, fives);
        }
        return hashes[seed];
    }

    static char? Find3(Dictionary<int, (string hash, char? three, List<char> fives)> hashes, string salt, int seed, int repeat = 0)
    {
        (var _, var three, var _) = GetHash(hashes, salt, seed, repeat);
        return three;

    }
    static bool IsValid2(Dictionary<int, (string hash, char? three, List<char> fives)> hashes, string salt, int seed, char c, int repeat = 0)
    {
        (var _, var _, var fives) = GetHash(hashes, salt, seed, repeat);
        return (fives.Contains(c)) ; 
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
        Dictionary<int, (string hash, char? three, List<char> fives)> hashes = new();
        var q = (
            from i in Range(0, int.MaxValue)
            let repeat = Find3(hashes, salt, i, 2016)
            where repeat.HasValue
            from j in Range(i + 1, 1000).SkipWhile(j => !IsValid2(hashes, salt, j, repeat.Value, 2016)).Take(1)
            select (i, repeat, j)
            ).Take(65);

        {
            int n = 1;
            int index = 0;
            foreach (var item in q)
            {
                var hash1 = GetHash(hashes, salt, item.i);
                var hash2 = GetHash(hashes, salt, item.j);
                Console.WriteLine($"{n++}: {item} ({hash1.hash}, {hash1.three}, {hash2.hash}, {new string(hash2.fives.ToArray())})");
                index = item.i;
            }
            return index;
        }
    }

    [Fact]
    public void Test()
    {
        Dictionary<int, (string hash, char? three, List<char> fives)> hashes = new();
            var salt = "abc";
        var index = 0;
        var hash = GetHash(hashes, salt, index, 2016);
        Assert.Equal(hash.hash, "a107ff634856bb300138cac6568c0f24");
    }

    [Fact]
    public void Test2()
    {
        Dictionary<int, (string hash, char? three, List<char> fives)> hashes = new();
        var salt = "abc";
        var hash22551 = GetHash(hashes, salt, 22551, 2016);
        var hash22859 = GetHash(hashes, salt, 22859, 2016);
        Assert.Contains("fff", hash22551.hash);
        Assert.Contains("fffff", hash22859.hash);
    }
}