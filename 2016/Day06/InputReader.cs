using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Jeroen
{

    public static class InputReader
    {
        public static IEnumerable<string> Test()
        {
            return ReadInput("test");
        }
        public static IEnumerable<string> Actual()
        {
            return ReadInput("real");
        }

        static IEnumerable<string> ReadInput(string kind)
        {
            using (var stream = File.OpenRead($"input.{kind}.txt"))
            using (var reader = new StreamReader(stream))
            {
                while (reader.Peek() >= 0) yield return reader.ReadLine();
            }
        }
    }
}
