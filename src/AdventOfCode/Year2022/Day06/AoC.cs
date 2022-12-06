namespace AdventOfCode.Year2022.Day06;
public class AoC202206
{
    static StreamReader Input() => Read.InputStream();
    public int Part1() => Find(Input(), 4);
    public int Part2() => Find(Input(), 14);

    private static int Find(StreamReader input, int size)
    {
        var q = new Queue<char>();
        int position = 1;
        while (input.Peek() >= 0)
        {
            var c = (char)input.Read();
            q.Enqueue(c);
            if (q.Count > size)
                q.Dequeue();
            if (q.Count() == size && q.Distinct().Count() == size)
            {
                return position;
            }
            position++;
        }
        return 0;
    }
}
