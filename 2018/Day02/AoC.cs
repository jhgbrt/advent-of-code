using System.Linq;

namespace AdventOfCode
{
    public static class AoC
    {
        public static int Part1(string[] input)
        {
            var (twos, threes) = input.Aggregate((twos: 0, threes: 0), (t, s) => Process(t.twos, t.threes, s));
            return twos * threes;
        }

        public static string Part2(string[] input)
        {
            var result = (
                from l in input
                from r in input
                let diffCount = DiffCount(l, r)
                where diffCount > 0
                select (l, r, diffCount)
                ).Aggregate((curMin, x) => x.diffCount < curMin.diffCount ? x : curMin);
            return Common(result.l, result.r);
        }
        static (int twos, int threes) Process(int twos, int threes, string s)
        {
            var g = from c in s group c by c;
            if (g.Any(x => x.Count() == 2)) twos++;
            if (g.Any(x => x.Count() == 3)) threes++;
            return (twos, threes);
        }

        static string Common(this string left, string right) 
            => new string((
                from x in left.Zip(right, (l, r) => (l, r))
                where x.l == x.r
                select x.l).ToArray());

        public static int DiffCount(this string left, string right) 
            => left.Zip(right, (l, r) => (l, r))
            .Aggregate(0, (int count, (char l, char r) x) => count += x.l == x.r ? 0 : 1);

    }
}
