using static AdventOfCode.Year2018.Day08.AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2018.Day08
{
    partial class AoC
    {
        static string input = File.ReadAllText("input.txt");

        internal static Result Part1() => Run(() => Part1(input));
        internal static Result Part2() => Run(() => Part2(input));

        public static int Part1(string input) => ToTree(input).AllNodes().SelectMany(n => n.MetaData).Sum();

        public static int Part2(string input) => ToTree(input).GetValue();

        static Node ToTree(string input)
        {
            var enumerator = input.ToIntegers().GetEnumerator();
            var root = ReadNode(enumerator);
            return root;
        }

        static Node ReadNode(IEnumerator<int> enumerator)
        {
            var nofchildren = enumerator.Next();
            var nofmetadata = enumerator.Next();
            var children = Enumerable.Range(0, nofchildren).Select(i => ReadNode(enumerator)).ToList();
            var metadata = enumerator.Read(nofmetadata).ToList();
            return new Node(children, metadata);
        }

    }
}

