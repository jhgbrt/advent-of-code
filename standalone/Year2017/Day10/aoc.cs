var input = "206,63,255,131,65,80,238,157,254,24,133,2,16,0,1,3";
var sw = Stopwatch.StartNew();
var part1 = Part1();
var part2 = KnotsHash.Hash(input);
Console.WriteLine((part1, part2, sw.Elapsed));
object Part1()
{
    var result = KnotsHash.Hash(input.Split(',').Select(byte.Parse).ToArray());
    var value = result[0] * result[1];
    return value;
}