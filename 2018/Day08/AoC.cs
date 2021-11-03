using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode
{

    static class AoC
    {
        public static int Part1(TextReader input) => ToTree(input).AllNodes().SelectMany(n => n.MetaData).Sum();

        public static int Part2(TextReader input) => ToTree(input).GetValue();

        static Node ToTree(TextReader input)
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

        static T Next<T>(this IEnumerator<T> enumerator)
        {
            enumerator.MoveNext();
            return enumerator.Current;
        }

        static IEnumerable<T> Read<T>(this IEnumerator<T> enumerator, int n) 
            => Enumerable.Range(0, n).Select(i => enumerator.Next());

        public static IEnumerable<int> ToIntegers(this TextReader input)
        {
            var sb = new StringBuilder();
            while (input.Peek() >= 0)
            {
                char c = (char)input.Read();
                if (char.IsDigit(c))
                {
                    sb.Append(c);
                }
                else
                {
                    yield return int.Parse(sb.ToString());
                    sb.Clear();
                }
            }
            if (sb.Length > 0)
                yield return int.Parse(sb.ToString());
        }
    }
}
