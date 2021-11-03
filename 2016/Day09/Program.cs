using System;
using System.IO;
using System.Linq;

namespace Jeroen
{

    public class Program
    {
        public static int Main(params string[] args)
        {
            var input = InputReader.ReadAllText();
            Console.WriteLine(input.GetDecompressedSize2(0, input.Length));
            return 0;
        }
    }
}
