
using static AdventOfCode.Year2019.Day07.AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2019.Day07
{
    partial class AoC
    {
        internal static string[] input = File.ReadAllLines("input.txt");

        internal static Result Part1() => Run(() =>
        {
            var program = Parse(input);
            foreach (var p in GetPermutations(Enumerable.Range(0, 5), 5))
            {
                int next;
                foreach (var i in p)
                {
                    next = IntCode.Run(program, i).Last();
                }


            }

            return 0;
        });
        internal static Result Part2() => Run(() => -1);

        static ImmutableArray<int> Parse(string[] input) => input[0].Split(',').Select(int.Parse).ToImmutableArray();
        internal static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length) => length == 1
               ? from t in list select Enumerable.Repeat(t, 1)
               : GetPermutations(list, length - 1).SelectMany(t => list.Where(e => !t.Contains(e)), (t1, t2) => t1.Concat(Enumerable.Repeat(t2, 1)).ToArray());

    }
}