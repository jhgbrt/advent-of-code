namespace AdventOfCode.Client;

using NodaTime;

record Answer(object? part1, object? part2) { public static Answer Empty => new Answer(null, null); }
record Puzzle(int Year, int Day, string Html, string Text, string Input, Answer Answer, Status Status)
{
    public bool Final => Status == Status.AnsweredPart2 || (Day == 25 && Status == Status.AnsweredPart1);
    public int Unanswered => Final ? 0 : Status == Status.AnsweredPart1 ? 1 : 2;
    public static Puzzle Locked(int year, int day) => new(year, day, string.Empty, string.Empty, string.Empty, Answer.Empty, Status.Locked);
    public static Puzzle Unlocked(int year, int day, string html, string text, string input, Answer answer) => new(year, day, html, text, input, answer, answer switch
    {
        { part1: null, part2: null} => Status.Unlocked,
        { part1: not null, part2: null } => Status.AnsweredPart1,
        { part1: not null, part2: not null} => Status.AnsweredPart2,
        _ => throw new Exception($"inconsistent state for {year}/{day}/{answer}")
    });
}
enum Status
{
    Locked,
    Unlocked,
    AnsweredPart1,
    AnsweredPart2
}

record LeaderBoard(int OwnerId, int Year, Member[] Members);
record Member(int Id, string Name, int TotalStars, int LocalScore, int GlobalScore, Instant LastStarTimeStamp, IReadOnlyDictionary<int, DailyStars> Stars);
record DailyStars(int Day, Instant? FirstStar, Instant? SecondStar);