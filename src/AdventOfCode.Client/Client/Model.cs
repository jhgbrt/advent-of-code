namespace AdventOfCode.Client;

using NodaTime;

enum ResultStatus
{
    NotImplemented, // not implemented
    Unknown,        // unknown if correct or not
    Failed,         // failed after verification
    Ok,              // correct after verification
    AnsweredButNotImplemented
}
record ComparisonResult(ResultStatus part1, ResultStatus part2)
{
    public static implicit operator bool(ComparisonResult result) => result.part1 != ResultStatus.Ok || result.part2 != ResultStatus.Ok;
}

record Answer(string part1, string part2) { public static Answer Empty => new Answer(string.Empty, string.Empty); }
record DayResult(int year, int day, Result part1, Result part2)
{
    public readonly static DayResult Empty = new DayResult(0, 0, Result.Empty, Result.Empty);
    public TimeSpan Elapsed => part1.Elapsed + part2.Elapsed;
}

record Result(ResultStatus Status, string Value, TimeSpan Elapsed)
{
    public readonly static Result Empty = new Result(ResultStatus.NotImplemented, string.Empty, TimeSpan.Zero);
    public Result Verify(string answer) => Status switch
    {
        ResultStatus.Unknown => this with { Status = answer == Value ? ResultStatus.Ok : ResultStatus.Failed },
        ResultStatus.NotImplemented when !string.IsNullOrEmpty(answer) => this with { Status = ResultStatus.AnsweredButNotImplemented },
        _ => this
    };
}

record Puzzle(int Year, int Day, string Html, string Text, string Input, Answer Answer, Status Status)
{
    public int Unanswered => Status switch { Status.Completed => 0, Status.AnsweredPart1 => 1, _ => 2 };
    public static Puzzle Locked(int year, int day) => new(year, day, string.Empty, string.Empty, string.Empty, Answer.Empty, Status.Locked);
    public static Puzzle Unlocked(int year, int day, string html, string text, string input, Answer answer) => new(year, day, html, text, input, answer, answer switch
    {
        { part1: null, part2: null} => Status.Unlocked,
        { part1: not null, part2: null } => day < 25 ? Status.AnsweredPart1 : Status.Completed,
        { part1: not null, part2: not null} => Status.Completed,
        _ => throw new Exception($"inconsistent state for {year}/{day}/{answer}")
    });

    public ComparisonResult Compare(DayResult result)
    {
        if ((result.year, result.day) != (Year, Day)) throw new InvalidOperationException("Result is for another day");

        return Day switch
        {
            25 => new ComparisonResult(result.part1.Verify(Answer.part1).Status, ResultStatus.Ok),
            _ => new ComparisonResult(result.part1.Verify(Answer.part1).Status, result.part2.Verify(Answer.part2).Status)
        };
    }

}
enum Status
{
    Locked,
    Unlocked,
    AnsweredPart1,
    Completed
}

record LeaderBoard(int OwnerId, int Year, Member[] Members);
record Member(int Id, string Name, int TotalStars, int LocalScore, int GlobalScore, Instant? LastStarTimeStamp, IReadOnlyDictionary<int, DailyStars> Stars);
record DailyStars(int Day, Instant? FirstStar, Instant? SecondStar);