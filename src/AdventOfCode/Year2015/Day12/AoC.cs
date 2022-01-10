using Newtonsoft.Json.Linq;

namespace AdventOfCode.Year2015.Day12;

public class AoC201512
{
    static string input = Read.InputText();

    public object Part1() => Traverse(Root(), false);
    public object Part2() => Traverse(Root(), true);

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