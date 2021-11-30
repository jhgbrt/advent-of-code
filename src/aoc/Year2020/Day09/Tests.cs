namespace AdventOfCode.Year2020.Day09;

public class Tests
{
    static long[] input = File.ReadLines("example.txt").Select(long.Parse).ToArray();
    public void Test1() => Assert.Equal(127, input.InvalidNumbers(5).First());
    public void Test2() => Assert.Equal(62, input.FindEncryptionWeakness(127));
}