using System;
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
            using (var stream = GetStream(kind))
            using (var reader = new StreamReader(stream))
            {
                while (reader.Peek() >= 0) yield return reader.ReadLine();
            }
        }

        public static Stream GetStream(string kind)
        {
            return File.OpenRead($"input.{kind}.txt");
        }

        public static string ReadAllText()
        {
            using (var stream = File.OpenRead($"input.real.txt"))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
