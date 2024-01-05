using AdventOfCode.Common;

using System.Runtime.CompilerServices;

namespace AdventOfCode.Year2016.Day14;

public class AoC201614
{
    //readonly string salt = "abc";
    readonly string salt = Read.InputLines()[0];
    string CurrentPath([CallerFilePath] string path = "") => Path.GetDirectoryName(path) ?? "";

    public int Part1() => FindHash64(GetHashes(50000, 0));

    public int Part2() => FindHash64(GetHashes(50000, 2016));

    public int FindHash64(IReadOnlyDictionary<int, string> hashes) => (
            from i in Range(1, int.MaxValue)
            let repeat = Find3(hashes[i])
            where repeat.HasValue
            from j in Range(i + 1, 1000).SkipWhile(j => !IsValid(hashes[j], repeat.Value)).Take(1)
            select i
            ).Take(64).Last();

    IReadOnlyDictionary<int,string> GetHashes(int n, int repeat)
    {
        var path = Path.Combine(CurrentPath(), $"hashes-{salt}-{repeat}.txt");
        if (!File.Exists(path))
        {
            using var s = new StreamWriter(File.OpenWrite(path));
            foreach (var i in Range(0, n))
            {
                var hash = ComputeHash(salt, i, repeat);
                s.WriteLine(hash);
            }
        }
        return File.ReadLines(path).Select((line, i) => (line, i)).ToDictionary(x => x.i, x => x.line);
    }
    
    static string ComputeHash(string salt, int seed, int repeat = 0)
    {
        var key = $"{salt}{seed}";
        var hash = MD5Hash.Compute(key);
        for (int i = 0; i < repeat; i++)
        {
            hash = MD5Hash.Compute(hash);
        }
        return hash;
    }

    static char? Find3(string hash) => (from i in Range(0, hash.Length - 2)
                                        let a = hash[i]
                                        let b = hash[i+1]
                                        where a == b
                                        let c = hash[i+2]
                                        where c == a
                                        select (char?)a).FirstOrDefault();
    static bool IsValid(string hash, char c) 
        => Range(0, hash.Length - 4).Any(i => hash[i] == c
                                        && hash[i + 1] == c
                                        && hash[i + 2] == c
                                        && hash[i + 3] == c
                                        && hash[i + 4] == c);
}