namespace AdventOfCode.Year2024.Day09;

public class AoC202409(string input)
{
    public AoC202409() : this(Read.InputLines()[0]) {}

    public long Part1()
    {
        var blocks = ParseInput(input);
        Compact1(blocks);
        return Checksum(blocks);
    }

    public long Part2()
    {
        var blocks = ParseInput(input);
        Compact2(blocks);
        return Checksum(blocks);
    }

    private long Checksum(FileBlock[] blocks)
        => (from b in blocks.Index() where b.Item.IsFile select (long)b.Index * b.Item.Id).Sum();


    public static FileBlock[] ParseInput(string input)
    {
        int totalLength = input.Sum(c => c - '0');

        var blocks = new FileBlock[totalLength];

        var (fileId, position, isfile) = (0, 0, true);

        foreach (char c in input)
        {
            int length = c - '0';
            for (int i = 0; i < length; i++)
            {
                blocks[position++] = isfile ? FileBlock.File(fileId) : FileBlock.Empty;
            }
            if (isfile) fileId++;
            isfile = !isfile;
        }

        return blocks;
    }

    private static void Compact1(Span<FileBlock> blocks)
    {
        int length = blocks.Length;
        var (left, right) = (0, length - 1);

        while (left < right)
        {
            // find leftmost free space and rightmost file
            while (left < length && blocks[left].IsFile) left++;
            while (right > left && !blocks[right].IsFile) right--;
            if (left < right)
            {
                blocks[left] = blocks[right];
                blocks[right] = FileBlock.Empty;
            }
        }
    }
    private static void Compact2(FileBlock[] blocks)
    {
        var ranges = from fileRange in GetFileRanges(blocks)
                     let free = FindFittingFreeBlock(blocks, fileRange)
                     where free.HasValue
                     select (fileRange, freeRange: free.Value);

        var span = blocks.AsSpan();
        foreach (var (file, free) in ranges)
        {
            span[file].CopyTo(span[free]);
            span[file].Fill(FileBlock.Empty);
        }
    }


    private static Range? FindFittingFreeBlock(ReadOnlySpan<FileBlock> blocks, Range file)
    {
        // find a large enough free block from the left, before this file
        var (fileStart, fileLength) = file.GetOffsetAndLength(blocks.Length);

        for (int i = 0; i < fileStart; i++)
        {
            var start = i;
            while (!blocks[i].IsFile && i < fileStart) i++;
            if (i - start >= fileLength) return start..i;
        }

        return null;
    }


    private static IEnumerable<Range> GetFileRanges(FileBlock[] blocks)
    {
        // find ranges for each file block, starting from the right
        // skip any free blocks and return each file block as a range
        for (int i = blocks.Length - 1; i >= 0; i--)
        {
            while (i >= 0 && !blocks[i].IsFile) i--;
            if (i < 0) break;
            var end = i + 1;
            var id = blocks[i].Id;
            while (i >= 0 && blocks[i].Id == id) i--;
            var start = i + 1;
            yield return start..end;
            i = start;
        }
    }
}


public class AoC202409Tests
{
    private readonly AoC202409 sut;
    public AoC202409Tests(ITestOutputHelper output)
    {
        var input = Read.SampleText();
        sut = new AoC202409(input);
    }

    [Fact]
    public void TestParsing()
    {
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(1928, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(2858, sut.Part2());
    }
}


public record struct FileBlock(int Id)
{
    public readonly bool IsFile => Id >= 0;
    public static readonly FileBlock Empty = new(-1);
    public static FileBlock File(int id) => new (id);
}

