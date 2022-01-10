namespace AdventOfCode.Year2016.Day04;

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