var input = "1113222113";
var sw = Stopwatch.StartNew();
var part1 = Run(input, 40);
var part2 = Run(input, 50);
Console.WriteLine((part1, part2, sw.Elapsed));
int Run(string input, int times)
{
    var sb = new StringBuilder();
    for (int i = 0; i < times; i++)
    {
        sb.Clear();
        char last = default;
        int n = 0;
        foreach (var c in input)
        {
            if (last == c)
            {
                n++;
            }
            else
            {
                if (last != default)
                    sb.Append(n).Append(last);
                last = c;
                n = 1;
            }
        }

        sb.Append(n).Append(last);
        input = sb.ToString();
    }

    return sb.Length;
}