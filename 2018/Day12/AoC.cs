using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode
{
    static class AoC
    {
        public static int Part1(string[] input, long generations)
        {
            string initialState = input[0].Substring(15);
            (string, char)[] rules = input.Skip(2).Select(line => line.Split(" => ")).Select(c => (c[0], c[1][0])).ToArray();
            return Calculate(generations, initialState, rules);
        }

        private static int Calculate(long generations, string initialState, (string, char)[] rules)
        {
            var zero = 0;
            string result = initialState;
            for (long i = 0; i < generations; i++)
            {
                if (result[0..5].Contains('#') || result[^5..^0].Contains('#'))
                {
                    result = "....." + result + ".....";
                    zero += 5;
                }
                result = Transform(result, rules);
            }

            return result.Select((c, i) => (c, n: i - zero)).Where(x => x.c == '#').Select(c => c.n).Sum();
        }

        public static string Transform(string input, (string pattern, char r)[] rules)
        {
            char[] result = Enumerable.Repeat('.', input.Length).ToArray();

            var q = from r in rules
                    from i in Enumerable.Range(0, input.Length - r.pattern.Length)
                    where r.pattern.SequenceEqual(input.Skip(i).Take(r.pattern.Length))
                    select (i: i + 2, c: r.r);
            foreach (var x in q)
            {
                result[x.i] = x.c;
            }
            return new string(result);
        }
        public static long Part2(string[] input)
        {
            string initialState = input[0].Substring(15);
            (string, char)[] rules = input.Skip(2).Select(line => line.Split(" => ")).Select(c => (c[0], c[1][0])).ToArray();

            var n = 200;
            long sum = 0;
            while (true)
            {
                var calculations = Enumerable.Range(0, 3).Select(i => Calculate(n + i, initialState, rules)).ToList();
                sum = calculations[0];
                var diffs = calculations.Zip(calculations.Skip(1)).Select(x => x.Second - x.First);
                if (diffs.Distinct().Count() == 1)
                    break;
                n += 100;
            }
            return sum + (50_000_000_000 - n) * 75;



        }

    }

}