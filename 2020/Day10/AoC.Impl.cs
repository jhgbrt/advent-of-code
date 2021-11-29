namespace AdventOfCode.Year2020.Day10;

partial class AoC
{
    static int[] input = new[] { 97, 62, 23, 32, 51, 19, 98, 26, 90, 134, 73, 151, 116, 76, 6, 94, 113, 127, 119, 44, 115, 50, 143, 150, 86, 91, 36, 104, 131, 101, 38, 66, 46, 96, 54, 70, 8, 30, 1, 108, 69, 139, 24, 29, 77, 124, 107, 14, 137, 16, 140, 80, 68, 25, 31, 59, 45, 126, 148, 67, 13, 125, 53, 57, 41, 47, 35, 145, 120, 12, 37, 5, 110, 138, 130, 2, 63, 83, 22, 79, 52, 7, 95, 58, 149, 123, 89, 109, 15, 144, 114, 9, 78 };
    static int[] Order(int[] array) => (
        from i in new[] { 0 }.Concat<int>(array).Concat<int>(new[] { array.Max() + 3 }) orderby i select i
    ).ToArray<int>();
    static IEnumerable<int> Differences(int[] input)
    {
        var ordered = Order(input);
        return from pair in ordered.Zip(ordered).Skip(1)
               select pair.Second - pair.First;
    }

    internal static Result Part1() => Run(() => Part1(input));
    internal static Result Part2() => Run(() => Part2(input));

    static int Part1(int[] input)
    {
        var array = input.ToArray();

        var ordered = (
            from i in new[] { 0 }.Concat<int>(array).Concat<int>(new[] { array.Max() + 3 }) orderby i select i
            ).ToArray<int>();

        var differences = from pair in ordered.Zip(ordered.Skip(1))
                          select pair.Second - pair.First;

        var part1 = differences.Count(d => d == 1) * differences.Count(d => d == 3);
        return part1;
    }
    static long Part2(int[] input)
    {
        var array = input.ToArray();

        var ordered = (
            from i in new[] { 0 }.Concat<int>(array).Concat<int>(new[] { array.Max() + 3 }) orderby i select i
            ).ToArray<int>();

        var differences = from pair in ordered.Zip(ordered.Skip(1))
                          select pair.Second - pair.First;

        var part2 = differences.FindNofConsecutiveOnes().Aggregate(1L, (x, y) => x * y);
        return part2;
    }

}