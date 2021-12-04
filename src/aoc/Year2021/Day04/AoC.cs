namespace AdventOfCode.Year2021.Day04;

public class AoC202104 : AoCBase
{
    static string[] input = Read.InputLines(typeof(AoC202104));
    static int[] numbers = input[0].Split(",").Select(int.Parse).ToArray();
    static IEnumerable<Board> ReadBoards() => input.Skip(2).GetBoards().ToArray();
    public override object Part1()
    {
        var boards = ReadBoards();

        foreach (var draw in numbers)
        {
            int i = 0;
            foreach (var board in boards)
            {
                board.Apply(draw);
                if (board.Won) return draw * board.Sum();
                i++;
            }
        }

        return -1;
    }
    public override object Part2()
    {
        var boards = ReadBoards().ToList();
        int lastwinning = 0;
        int winningdraw = 0;

        foreach (var draw in numbers)
        {
            foreach (var board in boards)
            {
                if (!board.Won)
                {
                    board.Apply(draw);
                    winningdraw = draw;
                    lastwinning = board.Sum();
                }
            }
        }


        return lastwinning * winningdraw;
    }
}


class Board
{
    int[,] _numbers;

    public Board(int[,] numbers)
    {
        _numbers = numbers;
    }

    public bool Apply(int number)
    {
        foreach (var c in Coordinates().Where(c => this[c] == number))
            this[c] = -1;
        return Won;
    }

    public bool Won
    {
        get
        {
            for (var row = 0; row < 5; row++)
                if (Range(0, 5).All(col => this[(row, col)] == -1)) return true;
            for (var col = 0; col< 5; col++)
                if (Range(0, 5).All(row => this[(row, col)] == -1)) return true;
            return false;
        }
    }
    int this[(int row, int col) p]
    {
        get => _numbers[p.row, p.col];
        set => _numbers[p.row, p.col] = value;
    }

    public int Sum() => Coordinates().Select(c => this[c]).Where(i => i != -1).Sum();

    static IEnumerable<(int row, int col)> Coordinates()
    {
        for (int row = 0; row < 5; row++)
            for (int col = 0; col < 5; col++)
                yield return (row, col);
    }
}



static class Extensions
{
    public static IEnumerable<Board> GetBoards(this IEnumerable<string> input) => from chunk in input.Chunk(6)
                                                                                   select CreateBoard(chunk.Take(5));

    static Board CreateBoard(IEnumerable<string> chunk)
    {
        var board = new int[5, 5];
        var row = 0;
        foreach  (var line in chunk)
        {
            var span = line.AsSpan();
            for (var col = 0; col < 5; col++)
            {
                board[row, col] = int.Parse(span.Slice(col*3, 2));
            }
            row++;
        }
        return new Board(board);
    }
}