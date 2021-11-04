using Newtonsoft.Json.Linq;

using Xunit;

using static AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

static class AoC
{
    static string input = File.ReadAllText("input.txt");

    public static object Part1() => Traverse(Root(), false);
    public static object Part2() => Traverse(Root(), true);
    
    static JToken Root()
    {
        var jobject = JObject.Parse("{\"root\": " + input + "}");
        var root = jobject["root"]!;
        return root;
    }

    static int Traverse(JToken o, bool removeRed) => o switch
    {
        JObject when removeRed && o.Children().OfType<JProperty>().Any(p => p.Children().OfType<JValue>().Any(v => v.Value<string>() == "red")) => 0,
        JValue v when int.TryParse(v.Value<string>(), out var i) => i,
        JValue => 0,
        _ => o.Children().Select(x => Traverse(x, removeRed)).Sum(),
    };

}

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal(191164, Part1());
    [Fact]
    public void Test2() => Assert.Equal(87842, Part2());
}

