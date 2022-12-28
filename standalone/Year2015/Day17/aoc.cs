var input = new[] { 43, 3, 4, 10, 21, 44, 4, 6, 47, 41, 34, 17, 17, 44, 36, 31, 46, 9, 27, 38 };
var sw = Stopwatch.StartNew();
var part1 = Part1(input, 150);
var part2 = Part2(input, 150);
Console.WriteLine((part1, part2, sw.Elapsed));
int Part1(int[] input, int sum) => Combinations(input).Count(c => c.Sum() == sum);
int Part2(int[] input, int sum)
{
    var array = Combinations(input).OrderBy(c => c.Length).Where(c => c.Sum() == sum).ToArray();
    var minlength = array.First().Length;
    return array.Count(c => c.Length == minlength && c.Sum() == sum);
}

IEnumerable<T[]> Combinations<T>(T[] data) => Range(0, 1 << (data.Length)).Select(index => data.Where((v, i) => (index & (1 << i)) != 0).ToArray());