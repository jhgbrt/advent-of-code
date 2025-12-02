namespace Net.Code.AdventOfCode.Toolkit.Core;

class Puzzle : IHavePuzzleKey
{
    public PuzzleKey Key { get; init; }
    public int Year { get; init; }
    public int Day { get; init; }
    public string Input { get; private set; }
    public Answer Answer { get; set; }
    public Status Status { get; set; }
    public string Html { get; set; } = string.Empty;

    public Puzzle(PuzzleKey key, string input, Answer answer, Status status, string html)
    {
        Key = key;
        Year = key.Year;
        Day = key.Day;
        Input = input;
        Answer = answer;
        Status = status;
        Html = html;
    }
    private Puzzle()
    {
        Input = string.Empty;
        Answer = Answer.Empty;
    }

    public static Puzzle Locked(PuzzleKey key) => new(key, string.Empty, Answer.Empty, Status.Locked, string.Empty);
    public static Puzzle Create(PuzzleKey key, string input, Answer answer, string html) => new(key, input, answer, answer switch
    {
        { part1: "", part2: "" } => Status.Unlocked,
        { part1: not "", part2: "" } => key.Day < 25 ? Status.AnsweredPart1 : Status.Completed,
        { part1: not "", part2: not "" } => Status.Completed,
        _ => throw new Exception($"inconsistent state for {key}/{answer}")
    }, html);

    public AnswerToPost CreateAnswer(string answer) => Status switch
    {
        Status.Locked => throw new PuzzleLockedException("Puzzle is locked. Did you initialize it?"),
        Status.Completed => throw new AlreadyCompletedException("Already completed"),
        Status.Unlocked => new(1, answer),
        Status.AnsweredPart1 => new(2, answer),
        _ => throw new NotSupportedException()
    };

    public void SetAnswer(AnswerToPost answer)
    {
        (Status, Answer) = answer.part switch
        {
            1 => (Status.AnsweredPart1, Answer with { part1 = answer.value }),
            2 => (Status.Completed, Answer with { part2 = answer.value }),
            _ => throw new NotSupportedException()
        };
    }

    internal void UpdateFrom(Puzzle remote)
    {
        Answer = remote.Answer;
        Status = remote.Status;
        Input = remote.Input;
        Html = remote.Html;
    }
}
