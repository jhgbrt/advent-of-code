namespace AdventOfCode.Year2024.Day19;

public class AoC202419
{
    public AoC202419() : this(Read.InputLines()) {}

    readonly string[] patterns;
    readonly string[] designs;
    readonly long[] cache;
    readonly Trie trie;

    public AoC202419(string[] input)
    {
        patterns = input[0].Split(", ");
        designs = input[2..];
        cache = new long[designs.Max(d => d.Length)];
        trie = new Trie(patterns);
    }
    public int Part1() => Counts().Where(c => c > 0).Count();

    public long Part2() => Counts().Sum();

    IEnumerable<long> Counts()
    {
        foreach (var design in designs)
        {
            Array.Clear(cache);
            var count = PatternMatch(design);
            yield return count;
        }
    }

    long PatternMatch(ReadOnlySpan<char> word)
    {
        if (word.Length == 0) return 1;

        int index = word.Length - 1;
        if (cache[index] > 0) return cache[index];

        long result = 0;

        var node = trie.Root;
        for (int i = 0; i < word.Length && node[word[i]] is not null; i++)
        {
            node = node[word[i]];
            if (node.IsEndOfWord)
            {
                result += PatternMatch(word[(i + 1)..]);
            }
        }

        cache[index] = result;
        return result;
    }
}

public class AoC202419Tests
{
    private readonly AoC202419 sut;
    public AoC202419Tests(ITestOutputHelper output)
    {
        var input = Read.SampleLines();
        sut = new AoC202419(input);
    }

    [Fact]
    public void TestParsing()
    {
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(6, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(16, sut.Part2());
    }
}

// https://en.wikipedia.org/wiki/Trie
class Trie
{
    private readonly Node root = new();
    public Node Root => root;

    public Trie(IEnumerable<string> patterns)
    {
        foreach (var pattern in patterns)
        {
            var node = root;
            foreach (var c in pattern)
            {
                node = node[c] ??= new();
            }
            node.IsEndOfWord = true;
        }
    }

   
    public class Node
    {
        public Node this[char c] { get => Children[c - 'a']; set => Children[c - 'a'] = value; }
        public Node[] Children { get; } = new Node[26];
        public bool IsEndOfWord { get; set; }
    }

}

