
var input = new[] { 43, 3, 4, 10, 21, 44, 4, 6, 47, 41, 34, 17, 17, 44, 36, 31, 46, 9, 27, 38 };
var sample = new[] { 20, 15, 10, 5, 5 };

Console.WriteLine(Part1(sample, 25));
Console.WriteLine(Part2(input, 25));
Console.WriteLine();
Console.WriteLine(Part1(input, 150));
Console.WriteLine(Part2(input, 150));


int Part1(int[] input, int sum) => Combinations(input).Where(c => c.Sum() == sum).Count();
int Part2(int[] input, int sum)
{
    var array = Combinations(input).OrderBy(c => c.Length).Where(c => c.Sum() == sum).ToArray();
    var minlength = array.First().Length;
    return array.Where(c => c.Length == minlength && c.Sum() == sum).Count();
};



static IEnumerable<T[]> Combinations<T>(T[] data) => Enumerable
      .Range(0, 1 << (data.Length))
      .Select(index => data.Where((v, i) => (index & (1 << i)) != 0).ToArray());