using Newtonsoft.Json.Linq;

namespace AdventOfCode.Year2022.Day13;
public class AoC202213
{
    static string[] input = Read.InputLines();
    static IComparer<JToken> comparer = new JTokenComparer();
    public int Part1() => Parse(input)
        .Select((p, index) => (p.left, p.right, result: comparer.Compare(p.left, p.right), index: index + 1))
        .Where(item => item.result > 0)
        .Select(item => item.index)
        .ToList()
        .Sum();

    static JToken[] dividers = new[]
    {
        Packet("[[2]]"),
        Packet("[[6]]")
    };

    public int Part2() => Parse(input)
        .SelectMany(item => new[] { item.left, item.right })
        .Concat(dividers)
        .ToImmutableSortedSet(comparer)
        .Reverse()
        .Select((p, i) => (p, index: i + 1))
        .Join(dividers, outer => outer.p, p => p, (outer, _) => outer.index)
        .Aggregate(1, (l, r) => l * r);

    IEnumerable<(JToken left, JToken right)> Parse(IEnumerable<string> input)
    {
        int i = 0;
        var enumerator = input.GetEnumerator();
        while (true)
        {
            i++;
            if (!enumerator.MoveNext()) yield break;
            var left = enumerator.Current;
            if (!enumerator.MoveNext()) yield break;
            var right = enumerator.Current;
            yield return (Packet(left), Packet(right));
            if (!enumerator.MoveNext()) yield break;
        }
    }


    static JToken Packet(string line) => JObject.Parse($"{{\"root\": {line}}}")["root"]!;

}

class JTokenComparer : IComparer<JToken>
{
    public int Compare(JToken? left, JToken? right) => (left!.Type, right!.Type) switch
    {
        (JTokenType.Array, JTokenType.Array) => CompareArrays(left.Value<JArray>()!, right.Value<JArray>()!),
        (JTokenType.Array, JTokenType.Integer) => Compare(left, Array(right.Value<int>())),
        (JTokenType.Integer, JTokenType.Array) => Compare(Array(left.Value<int>()), right),
        (JTokenType.Integer, JTokenType.Integer) => Compare(left.Value<int>(), right.Value<int>()),
        _ => 0
    };

    int CompareArrays(JArray l, JArray r)
    {
        var left = l.Children().ToArray();
        var right = r.Children().ToArray();

        for (int i = 0; i < Math.Min(left.Length, right.Length); i++)
        {
            var result = Compare(left[i], right[i]);
            if (result != 0)
            {
                return result;
            }
        }

        return Compare(left.Length, right.Length);
    }

    static JToken Array(int i) => JToken.Parse($"[{i}]");

    static int Compare(int left, int right) => (right - left) switch
    {
        < 0 => -1,
        0 => 0,
        > 0 => 1
    };


}