namespace AdventOfCode;

public static class StringBuilderExtensions
{
    public static int IndexOf(this StringBuilder self, char c)
    {
        for (int i = 0; i < self.Length; i++)
        {
            if (self[i] == c)
            {
                return i;
            }
        }
        return -1;
    }
}
