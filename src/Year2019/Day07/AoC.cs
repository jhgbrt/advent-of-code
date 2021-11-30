namespace AdventOfCode.Year2019.Day07;

public class AoCImpl : AoCBase
{
    internal static string[] input = Read.InputLines(typeof(AoCImpl));

    public override object Part1()
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

        return -1;
    }
    public override object Part2() => -1;

    static ImmutableArray<int> Parse(string[] input) => input[0].Split(',').Select(int.Parse).ToImmutableArray();
    internal static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length) => length == 1
           ? from t in list select Enumerable.Repeat(t, 1)
           : GetPermutations(list, length - 1).SelectMany(t => list.Where(e => !t.Contains(e)), (t1, t2) => t1.Concat(Enumerable.Repeat(t2, 1)).ToArray());

}