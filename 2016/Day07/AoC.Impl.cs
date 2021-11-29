namespace AdventOfCode.Year2016.Day07;

partial class AoC
{
    static bool test = false;
    public static string[] input = File.ReadAllLines(test ? "sample.txt" : "input.txt");

    internal static Result Part1() => Run(() => input.Select(s => new IPAddress(s)).Where(ip => ip.SupportsTLS()).Count());
    internal static Result Part2() => Run(() => input.Select(s => new IPAddress(s)).Where(ip => ip.SupportsSSL()).Count());
}

enum WhereAmI
{
    Outside,
    Inside
}

struct IPAddress
{
    string input;

    public IPAddress(string input)
    {
        this.input = input;
    }
    public bool SupportsSSL()
    {
        var whereami = WhereAmI.Outside;
        var set1 = new HashSet<string>();
        var set2 = new HashSet<string>();
        bool atLeastOnePalindrome = false;
        var startIndex = 0;
        for (int i = 0; i < input.Length; i++)
        {
            var c = input[i];
            switch (whereami)
            {
                case WhereAmI.Outside when c == '[':
                    whereami = WhereAmI.Inside;
                    startIndex = i + 1;
                    break;
                case WhereAmI.Outside when i >= startIndex + 2:
                    if (IsAba(i))
                    {
                        var bab = new string(new[] { input[i - 1], input[i], input[i - 1] });
                        if (set2.Contains(bab)) return true;
                        set1.Add(input.Substring(i - 2, 3));
                    }
                    break;
                case WhereAmI.Inside when c == ']':
                    whereami = WhereAmI.Outside;
                    startIndex = i + 1;
                    break;
                case WhereAmI.Inside when i >= startIndex + 2:
                    if (IsAba(i))
                    {
                        var bab = new string(new[] { input[i - 1], input[i], input[i - 1] });
                        if (set1.Contains(bab)) return true;
                        set2.Add(input.Substring(i - 2, 3));
                    }
                    break;
            }
        }
        return atLeastOnePalindrome;
    }

    private bool IsAba(int i)
    {
        return input[i] == input[i - 2] && input[i - 1] != input[i];
    }

    public bool SupportsTLS()
    {
        var whereami = WhereAmI.Outside;

        bool atLeastOnePalindrome = false;
        var startIndex = 0;
        for (int i = 0; i < input.Length; i++)
        {
            var c = input[i];
            switch (whereami)
            {
                case WhereAmI.Outside when c == '[':
                    whereami = WhereAmI.Inside;
                    startIndex = i + 1;
                    break;
                case WhereAmI.Outside when !atLeastOnePalindrome && i >= startIndex + 3:
                    atLeastOnePalindrome = IsPalindrome(i);
                    break;
                case WhereAmI.Inside when c == ']':
                    whereami = WhereAmI.Outside;
                    startIndex = i + 1;
                    break;
                case WhereAmI.Inside when i >= startIndex + 3:
                    if (IsPalindrome(i)) return false;
                    break;
            }
        }
        return atLeastOnePalindrome;
    }

    private bool IsPalindrome(int i)
    {
        return (
            input[i] == input[i - 3] && input[i - 1] == input[i - 2] && input[i - 1] != input[i]
            );
    }
}
