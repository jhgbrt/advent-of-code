var sw = Stopwatch.StartNew();
var part1 = Find(Input(), 4);
var part2 = Find(Input(), 14);
Console.WriteLine((part1, part2, sw.Elapsed));
StreamReader Input() => new StreamReader(File.OpenRead("input.txt"));
int Find(StreamReader input, int size)
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