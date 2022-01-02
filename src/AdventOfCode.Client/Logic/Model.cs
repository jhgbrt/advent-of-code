namespace AdventOfCode.Client.Logic;

using NodaTime;

using System.ComponentModel.DataAnnotations;

enum ResultStatus
{
    [Display(Name="N/A")]
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
    public static DayResult NotImplemented(int year, int day) => new DayResult(year, day, Result.Empty, Result.Empty);
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
        { part1: "", part2: "" } => Status.Unlocked,
        { part1: not "", part2: "" } => day < 25 ? Status.AnsweredPart1 : Status.Completed,
        { part1: not "", part2: not "" } => Status.Completed,
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

record LeaderBoard(int OwnerId, int Year, IReadOnlyDictionary<int, Member> Members);
record Member(int Id, string Name, int TotalStars, int LocalScore, int GlobalScore, Instant? LastStarTimeStamp, IReadOnlyDictionary<int, DailyStars> Stars);
record DailyStars(int Day, Instant? FirstStar, Instant? SecondStar);


record ReportLine(int year, int day, ConsoleColor color, string status, ConsoleColor dcolor, string duration, string explanation)
{
    public override string ToString() => $"{year}-{day:00} [[[{color}]{status}[/]]] - [{dcolor}]{duration}[/]{explanation}";
}

record PuzzleResultStatus(Puzzle puzzle, DayResult result, bool refreshed)
{
    public ReportLine ToReportLine()
    {
        (var duration, var dcolor) = result.Elapsed.TotalMilliseconds switch
        {
            < 10 => ("< 10 ms", Console.ForegroundColor),
            < 100 => ("< 100 ms", Console.ForegroundColor),
            < 1000 => ("< 1s", Console.ForegroundColor),
            double value when value < 3000 => ($"~ {(int)Math.Round(value / 1000)} s", ConsoleColor.Yellow),
            double value => ($"~ {(int)Math.Round(value / 1000)} s", ConsoleColor.Red)
        };

        var comparisonResult = puzzle.Compare(result);

        (var status, var color, var explanation) = comparisonResult switch
        {
            { part1: ResultStatus.Failed } or { part2: ResultStatus.Failed } => ("FAILED", ConsoleColor.Red, $"- expected {(puzzle.Answer.part1, puzzle.Answer.part2)} but was ({(result.part1.Value, result.part2.Value)})."),
            { part1: ResultStatus.AnsweredButNotImplemented } or { part2: ResultStatus.AnsweredButNotImplemented } => ("SKIPPED", ConsoleColor.Red, " - answered but no implementation."),
            { part1: ResultStatus.NotImplemented, part2: ResultStatus.NotImplemented } => ("SKIPPED", ConsoleColor.Yellow, " - not implemented."),
            { part1: ResultStatus.NotImplemented, part2: ResultStatus.Ok } => ("SKIPPED", ConsoleColor.Yellow, " - part 1 not implemented."),
            { part1: ResultStatus.Ok, part2: ResultStatus.NotImplemented } => ("SKIPPED", ConsoleColor.Yellow, " - part 2 not implemented."),
            _ => ("OK", ConsoleColor.Green, "")
        };

        return new ReportLine(result.year, result.day, color, status, dcolor, duration, explanation);
    }
}
record LeaderboardEntry(string name, long score, long stars, DateTimeOffset lastStar);
record PuzzleReportEntry(
    int year, int day, string answer1, string answer2,
    string result1, TimeSpan elapsed1, ResultStatus status1,
    string result2, TimeSpan elapsed2, ResultStatus status2,
    TimeSpan elapsedTotal);

record MemberStats(string name, int stars, int score);