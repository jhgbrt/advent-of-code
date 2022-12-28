using System.Security.Cryptography;

var key = "bgvyzdsv";
var sw = Stopwatch.StartNew();
var part1 = Solve(key, 5);
var part2 = Solve(key, 6);
Console.WriteLine((part1, part2, sw.Elapsed));
int Solve(string key, int n)
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