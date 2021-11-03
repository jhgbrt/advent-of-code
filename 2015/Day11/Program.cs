using System.Diagnostics;

var password = "cqjxjnds".ToCharArray();

Debug.Assert(DoesNotContainOIL("abc".ToCharArray()));
Debug.Assert(!DoesNotContainOIL("i".ToCharArray()));
Debug.Assert(!DoesNotContainOIL("oasdf".ToCharArray()));
Debug.Assert(!DoesNotContainOIL("l".ToCharArray()));

Debug.Assert(ContainsTwoPairs("aabb".ToCharArray()));
Debug.Assert(ContainsTwoPairs("aabbccdd".ToCharArray()));
Debug.Assert(ContainsTwoPairs("aabbccdd".ToCharArray()));
Debug.Assert(!ContainsTwoPairs("abcdefg".ToCharArray()));
Debug.Assert(!ContainsTwoPairs("aaabcdef".ToCharArray()));

Debug.Assert(!IncludesStraightOfThree("cqjxxyyz".ToCharArray()));
Debug.Assert(!IncludesStraightOfThree("acbedgfh".ToCharArray()));
Debug.Assert(IncludesStraightOfThree("abcedgfh".ToCharArray()));

do
{
    password = Increment(password);
} while (!IsValid(password));
Console.WriteLine(new string(password));

do
{
    password = Increment(password);
} while (!IsValid(password));
Console.WriteLine(new string(password));


bool IsValid(char[] password) => DoesNotContainOIL(password) && ContainsTwoPairs(password) && IncludesStraightOfThree(password);

bool ContainsTwoPairs(char[] password) => (from chunk in password.Chunk(2).Concat(password.Skip(1).Chunk(2))
                                           where chunk.Length == 2 && chunk[0] == chunk[1]
                                           select (chunk[0], chunk[1])).Distinct().Count() >= 2;

bool IncludesStraightOfThree(char[] password) => (from zip in password.Zip(password.Skip(1), password.Skip(2))
                                                  let a = zip.First
                                                  let b = zip.Second
                                                  let c = zip.Third
                                                  where (a + 1 == b) && (b + 1 == c)
                                                  select (a, b, c)).Any();

bool DoesNotContainOIL(char[] password) => !new[] { 'o', 'i', 'l'}.Intersect(password).Any();

char[] Increment(char[] p)
{
    return (p[0], p[1], p[2], p[3], p[4], p[5], p[6], p[7]) switch
    {
        ('z', 'z', 'z', 'z', 'z', 'z', 'z', 'z') => throw new OverflowException(),
        (_, 'z', 'z', 'z', 'z', 'z', 'z', 'z') => new[] { (char)(p[0] + 1), 'a', 'a', 'a', 'a', 'a', 'a', 'a' },
        (_, _, 'z', 'z', 'z', 'z', 'z', 'z') => new[] { p[0], (char)(p[1] + 1), 'a', 'a', 'a', 'a', 'a', 'a' },
        (_, _, _, 'z', 'z', 'z', 'z', 'z') => new[] { p[0], p[1], (char)(p[2] + 1), 'a', 'a', 'a', 'a', 'a' },
        (_, _, _, _, 'z', 'z', 'z', 'z') => new[] { p[0], p[1], p[2], (char)(p[3] + 1), 'a', 'a', 'a', 'a' },
        (_, _, _, _, _, 'z', 'z', 'z') => new[] { p[0], p[1], p[2], p[3], (char)(p[4] + 1), 'a', 'a', 'a' },
        (_, _, _, _, _, _, 'z', 'z') => new[] { p[0], p[1], p[2], p[3], p[4], (char)(p[5] + 1), 'a', 'a' },
        (_, _, _, _, _, _, _, 'z') => new[] { p[0], p[1], p[2], p[3], p[4], p[5], (char)(p[6] + 1), 'a' },
        (_, _, _, _, _, _, _, _) => new[] { p[0], p[1], p[2], p[3], p[4], p[5], p[6], (char)(p[7] + 1) }
    };
}