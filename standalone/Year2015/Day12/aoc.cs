using Newtonsoft.Json.Linq;

var input = File.ReadAllText("input.txt");
var sw = Stopwatch.StartNew();
var part1 = Traverse(Root(), false);
var part2 = Traverse(Root(), true);
Console.WriteLine((part1, part2, sw.Elapsed));
JToken Root() => JObject.Parse("{\"root\": " + input + "}")["root"]!;
int Traverse(JToken o, bool removeRed) => o switch
{
    JObject when removeRed && o.Children().OfType<JProperty>().Any(p => p.Children().OfType<JValue>().Any(v => v.Value<string>() == "red")) => 0,
    JValue v when int.TryParse(v.Value<string>(), out var i) => i,
    JValue => 0,
    _ => o.Children().Select(x => Traverse(x, removeRed)).Sum(),
};
partial class AoCRegex
{
}