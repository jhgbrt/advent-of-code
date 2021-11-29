namespace AdventOfCode.Year2018.Day07;

class Graph
{
    public Graph(IEnumerable<(char from, char to)> edges)
    {
        Edges = edges.ToList();
        Vertices = edges.SelectMany(x => new[] { x.from, x.to }).OrderBy(c => c).Distinct().ToList();
    }

    public IReadOnlyList<(char from, char to)> Edges { get; }
    public IReadOnlyList<char> Vertices { get; }

    public IEnumerable<char> FindStepOrder()
    {
        var done = new HashSet<char>();
        var vertices = Vertices.Except(done);
        while (vertices.Any())
        {
            var edges = Edges.Where(e => !done.Contains(e.from));
            var step = vertices.Where(v => !edges.Any(e => e.to == v)).First();
            done.Add(step);
            yield return step;
        }
    }
    public int FindTotalDuration(int nofworkers, int offset)
    {
        var workers = new int[nofworkers];
        var working = new List<(char step, int finish)>();
        var done = new HashSet<char>();
        var vertices = Vertices.ToList();

        var ticks = 0;
        while (vertices.Any() || workers.Any(w => w > ticks))
        {
            foreach (var w in working.ToList().Where(d => d.finish <= ticks))
            {
                done.Add(w.step);
                working.Remove(w);
            }

            var edges = Edges.Where(e => !done.Contains(e.from));
            var steps = vertices.Where(v => !edges.Any(e => e.to == v)).ToList();

            for (var w = 0; w < workers.Length && steps.Any(); w++)
            {
                if (workers[w] > ticks) continue;
                var step = steps.First();
                workers[w] = step.GetTime(offset) + ticks;
                vertices.Remove(step);
                working.Add((step, workers[w]));
                steps.RemoveAt(0);
            }

            ticks++;
        }

        return ticks;
    }

}