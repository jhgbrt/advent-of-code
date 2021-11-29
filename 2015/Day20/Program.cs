using static AdventOfCode.Year2015.Day20.AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2015.Day20
{
    partial class AoC
    {
        static int input = 34000000;

        internal static Result Part1() => Run(() => Part1Impl());
        internal static Result Part2() => Run(() => Part2Impl());

        public static int Part1Impl()
        {
            var result = from house in Enumerable.Range(1, input)
                         let presents = (
                             from elve in Factors(house)
                             where house % elve == 0
                             select elve * 10
                         ).Sum()
                         where presents >= input
                         select (house, presents);
            return result.First(x => x.presents >= input).house;
        }
        public static int Part2Impl()
        {
            var deliverycount = new Dictionary<int, int>();
            foreach (int house in Enumerable.Range(1, input))
            {
                int presents = 0;
                foreach (var elve in Factors(house))
                {
                    if (!deliverycount.ContainsKey(elve))
                        deliverycount[elve] = 1;
                    else if (deliverycount[elve] <= 50)
                        deliverycount[elve]++;
                    if (deliverycount[elve] <= 50)
                        presents += elve * 11;
                }
                if (presents > input)
                    return house;
            }
            return 0;
        }

        static IEnumerable<int> Factors(int n)
        {
            for (int i = 1; i <= Math.Sqrt(n); i++)
            {
                if (n % i == 0)
                {
                    yield return i;
                    if (i != n / i)
                    {
                        yield return n / i;
                    }
                }
            }
        }
    }
}



/*
            House
    Elve    1  2  3  4  5  6  7  8  9  10 11 12 13
    1       x  x  x  x  x  x  x  x  x  x  x  x
    2          x     x     x  x  x     x     x
    3             x        x        x        x
    4                x           x           x
    5                   x              x
    6                      x                 x
    7                         x
    8                            x
    9                               x
   10                                  x
   11                                     x
   12                                        x

presents = (house 
 
 */

