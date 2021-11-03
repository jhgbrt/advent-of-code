using System.Text;

var input = "1113222113";

var sb = new StringBuilder();

for (int i = 0; i < 50; i++)
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
            if (last != default) sb.Append(n).Append(last);
            last = c;
            n = 1;
        }
    }
    sb.Append(n).Append(last);
    input = sb.ToString();
}

Console.WriteLine(sb.Length);