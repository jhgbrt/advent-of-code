using System;
using System.Linq;
using System.Threading.Tasks;

namespace Jeroen
{

    public class Program
    {
        public static int Main(params string[] args)
        {
            var query = from input in InputReader.Actual()
                        let ip = new IPAddress(input)
                        //where ip.SupportsTLS()
                        where ip.SupportsSSL()
                        select ip;
            
            Console.WriteLine(query.AsParallel().Count().ToString());
            return 0;
        }
    }
}
