namespace Jump
{
    public static class Jumps
    {
        public static int CalculateJumps(ReadOnlySpan<int> input, Func<int, int> step)
        {
            var copy = input.ToArray();
            var steps = 0;
            var i = 0;
            while (i >= 0 && i < input.Length)
            {
                steps++;
                var j = i;
                var v = copy[i];
                i += v;
                copy[j] += step(v);
            }
            return steps;
        }
    }
}