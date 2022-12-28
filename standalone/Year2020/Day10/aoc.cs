var input = new[] { 97, 62, 23, 32, 51, 19, 98, 26, 90, 134, 73, 151, 116, 76, 6, 94, 113, 127, 119, 44, 115, 50, 143, 150, 86, 91, 36, 104, 131, 101, 38, 66, 46, 96, 54, 70, 8, 30, 1, 108, 69, 139, 24, 29, 77, 124, 107, 14, 137, 16, 140, 80, 68, 25, 31, 59, 45, 126, 148, 67, 13, 125, 53, 57, 41, 47, 35, 145, 120, 12, 37, 5, 110, 138, 130, 2, 63, 83, 22, 79, 52, 7, 95, 58, 149, 123, 89, 109, 15, 144, 114, 9, 78 };
var sw = Stopwatch.StartNew();
var part1 = Part1(input);
var part2 = Part2(input);
Console.WriteLine((part1, part2, sw.Elapsed));
int[] Order(int[] array) => (
    from i in new[] { 0 }.Concat<int>(array).Concat<int>(new[] { array.Max() + 3 })
    orderby i
    select i).ToArray<int>();
IEnumerable<int> Differences(int[] input)
{
    var ordered = Order(input);
    return
        from pair in ordered.Zip(ordered).Skip(1) select pair.Second - pair.First;
}