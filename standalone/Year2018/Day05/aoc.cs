var input = File.ReadAllLines("input.txt").First();
var sw = Stopwatch.StartNew();
var part1 = Part1(input);
var part2 = Part2(input);
Console.WriteLine((part1, part2, sw.Elapsed));
int React(StringBuilder sb, string input, char? ignore)
{
    sb.Clear().Append(input);
    var length = sb.Length;
    sb = React(sb, ignore);
    while (sb.Length < length)
    {
        length = sb.Length;
        sb = React(sb, ignore);
    }

    var result = sb.Length;
    sb.Clear();
    return result;
}

StringBuilder React(StringBuilder input, char? ignore)
{
    var i = 0;
    while (i < input.Length - 1)
    {
        // char arithmetic turns out to be much faster than ToLowerInvariant/ToUpperInvariant
        var diff = 'a' - 'A';
        var c1 = input[i];
        var c2 = input[i + 1];
        //if (c1 == ignore || char.ToLowerInvariant(c1) == ignore)
        if (c1 == ignore || (c1 + diff) == ignore)
        {
            input.Remove(i, 1);
            continue;
        }

        //if (c1 != c2 && char.ToUpperInvariant(c1) == char.ToUpperInvariant(c2))
        if (c1 != c2 && Math.Abs(c1 - c2) == diff)
        {
            input.Remove(i, 2);
        }
        else
        {
            i++;
        }
    }

    return input;
}