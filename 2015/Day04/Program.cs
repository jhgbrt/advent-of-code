using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

var tests = new[]
{
    ("abcdef", 609043),
    ("pqrstuv", 1048970)
};

foreach (var test in tests)
{
    var solution = Solve(test.Item1, 5);
    Debug.Assert(solution == test.Item2);
}

var key = "bgvyzdsv";
Console.WriteLine(Solve(key, 5));
Console.WriteLine(Solve(key, 6));


static int Solve(string key, int n)
{
    var md5 = MD5.Create();
    var i = 0;
    while (true)
    {
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(key + i));
        var s = Convert.ToHexString(hash);
        if (s.Take(n).All(x => x == '0'))
        {
            return i;
        }
        i++;
    }
}
