using System.Security.Cryptography;

namespace AdventOfCode.Common
{
    internal static class MD5Hash
    {
        static MD5 md5 = MD5.Create();

        public static string Compute(string input)
        {
            var hash = md5.ComputeHash(Encoding.ASCII.GetBytes(input));
            StringBuilder sb = new();
            for (var i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString().ToLowerInvariant();
        }
    }
}
