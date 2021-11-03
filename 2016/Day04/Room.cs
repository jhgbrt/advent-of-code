using System;
using System.Linq;
using System.Text;

namespace Jeroen
{

    public partial class Tests
    {

        class Room
        {
            string _id;
            int _sectorId;
            string _checksum;

            public int SectorId => _sectorId;
            public string Id => _id;
            public string Checksum => _checksum ;

            private Room(string id, int sectorId, string checksum)
            {
                _id = id;
                _sectorId = sectorId;
                _checksum = checksum;
            }

            public static Room Parse(string s)
            {
                var sb = new StringBuilder();
                int i = 0;
                while (!char.IsDigit(s[i]))
                {
                    sb.Append(s[i]);
                    i++;
                }
                var id = sb.ToString(0, sb.Length - 1);
                sb.Clear();

                while (char.IsDigit(s[i]))
                {
                    sb.Append(s[i]);
                    i++;
                }
                var sectorId = int.Parse(sb.ToString());

                if (s[i] != '[') throw new FormatException();
                i++;

                var checksum = s.Substring(i, 5);
                return new Room(id, sectorId, checksum);
            }

            public bool IsValid()
            {
                var letters = _id
                    .Where(Char.IsLetter)
                    .GroupBy(c => c)
                    .Select(g => new { Count = g.Count(), c = g.First() })
                    .OrderByDescending(g => g.Count)
                    .ThenBy(g => g.c)
                    .Take(5)
                    .Select(g => g.c)
                    .ToArray();

                return new string(letters) == _checksum;
            }


            public string Name
            {
                get
                {
                    return _id.Decrypt(_sectorId);
                }
            }
        }
    }

    public static class Extensions
    {
        public static string Decrypt(this string encrypted, int rotations)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var c in encrypted)
            {
                if (c == '-') sb.Append(' ');
                else
                {
                    var offset = (c + rotations - 'a') % 26;
                    var result = (char)('a' + offset);
                    sb.Append(result);
                }
            }
            return sb.ToString();
        }
    }
}
