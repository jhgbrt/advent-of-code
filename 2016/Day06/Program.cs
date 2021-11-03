using System;

namespace Jeroen
{

    public class Program
    {
        public static int Main(params string[] args)
        {
            var result = new Accumulator().Decode(InputReader.Actual(), 8, true);
            Console.WriteLine(result);
            return 0;
        }
    }
}
