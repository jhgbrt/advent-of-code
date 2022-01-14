using Net.Code.AdventOfCode.Tool.Commands;
using Net.Code.AdventOfCode.Tool.Core;

using NSubstitute;

using Spectre.Console.Cli;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Xunit;

namespace Net.Code.AdventOfCode.Tool.UnitTests;

public class CommandTests
{
    [Fact]
    public async Task Init()
    {
        TestClock.SetClock(2021, 12, 26, 0, 0, 0);
        var manager = CreateCodeManager();
        var sut = new Init(manager);
        await sut.ExecuteAsync(2021, 1, new());
        await manager.Received(1).InitializeCodeAsync(2021, 1, false, Arg.Any<Action<string>>());
    }

    [Fact]
    public async Task Leaderboard_WithId()
    {
        TestClock.SetClock(2021, 12, 26, 0, 0, 0);
        var manager = CreateReportManager();
        var run = new Leaderboard(manager);
        await run.ExecuteAsync(new CommandContext(Substitute.For<IRemainingArguments>(), "leaderboard", default), new Leaderboard.Settings { year = 2021, id = 123 });
        await manager.Received(1).GetLeaderboardAsync(2021, 123);

    }

    [Fact]
    public async Task Leaderboard_NoId()
    {
        TestClock.SetClock(2021, 12, 26, 0, 0, 0);
        var manager = CreateReportManager();
        var run = new Leaderboard(manager);
        await run.ExecuteAsync(new CommandContext(Substitute.For<IRemainingArguments>(), "leaderboard", default), new Leaderboard.Settings { year = 2021 });
        await manager.Received(1).GetLeaderboardAsync(2021, 123);
    }

    [Fact]
    public async Task Run()
    {
        TestClock.SetClock(2021, 12, 26, 0, 0, 0);
        var manager = Substitute.For<IAoCRunner>();
        var run = new Run(manager);
        await run.ExecuteAsync(2021, 1, new());
        await manager.Received(1).Run(null, 2021, 1, Arg.Any<Action<int, Result>>());
    }
    [Fact]
    public async Task Verify()
    {
        TestClock.SetClock(2021, 12, 26, 0, 0, 0);
        IPuzzleManager manager = CreatePuzzleManager();
        var run = new Verify(manager);
        await run.ExecuteAsync(2021, 1, new());
        await manager.Received(1).GetPuzzleResult(2021, 1, Arg.Any<Action<int, Result>>());
    }

    [Fact]
    public async Task Sync()
    {
        TestClock.SetClock(2021, 12, 26, 0, 0, 0);
        var manager = CreatePuzzleManager();
        var sut = new Sync(manager);
        await sut.ExecuteAsync(2021, 1, new());
        await manager.Received(1).Sync(2021, 1);
    }

    [Fact]
    public async Task Export_NoOutput()
    {
        TestClock.SetClock(2021, 12, 26, 0, 0, 0);
        var manager = CreateCodeManager();
        var sut = new Export(manager);
        await sut.ExecuteAsync(2021, 1, new());
        await manager.Received(1).GenerateCodeAsync(2021, 1);
        await manager.DidNotReceive().ExportCode(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Export_Output()
    {
        TestClock.SetClock(2021, 12, 26, 0, 0, 0);
        var manager = CreateCodeManager();
        var sut = new Export(manager);
        await sut.ExecuteAsync(2021, 1, new Export.Settings { output = "output.txt" });
        await manager.Received(1).GenerateCodeAsync(2021, 1);
        await manager.Received(1).ExportCode(2021, 1, "public class AoC202101 {}", "output.txt");
    }

    [Fact]
    public async Task Post_WhenPuzzleIsValid()
    {
        TestClock.SetClock(2021, 12, 26, 0, 0, 0);
        var manager = CreatePuzzleManager();
        manager.PreparePost(Arg.Any<int>(), Arg.Any<int>()).Returns((true, "reason", 1));
        var sut = new Post(manager);
        await sut.ExecuteAsync(2021, 5, new Post.Settings { value = "SOLUTION" });
        await manager.Received().Post(2021, 5, 1, "SOLUTION");
    }

    [Fact]
    public async Task Post_WhenPuzzleIsInvalid()
    {
        TestClock.SetClock(2021, 12, 26, 0, 0, 0);
        var manager = CreatePuzzleManager();
        manager.PreparePost(Arg.Any<int>(), Arg.Any<int>()).Returns((false, "reason", 0));
        var sut = new Post(manager);
        await sut.ExecuteAsync(2021, 5, new Post.Settings { value = "SOLUTION" });
        await manager.DidNotReceive().Post(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Report()
    {
        TestClock.SetClock(2021, 12, 26, 0, 0, 0);
        var manager = CreateReportManager();
        var run = new Report(manager);
        await run.ExecuteAsync(new CommandContext(Substitute.For<IRemainingArguments>(), "report", default), new() );
        manager.Received().GetPuzzleReport(default, default);
    }

    [Fact]
    public async Task Stats()
    {
        TestClock.SetClock(2021, 12, 26, 0, 0, 0);
        var manager = CreateReportManager();
        var run = new Stats(manager);
        await run.ExecuteAsync(new CommandContext(Substitute.For<IRemainingArguments>(), "stats", default), default!);
        manager.Received().GetMemberStats();
    }

    private static IReportManager CreateReportManager()
    {
        var manager = Substitute.For<IReportManager>();
        manager.GetLeaderboardIds(Arg.Any<bool>()).Returns(new[] { (123, "") });
        return manager;
    }

    private static ICodeManager CreateCodeManager()
    {
        var manager = Substitute.For<ICodeManager>();
        foreach (var y in AoCLogic.Years())
            foreach (var d in Enumerable.Range(1, 25))
                manager.GenerateCodeAsync(y, d).Returns(
                    $"public class AoC{y}{d:00} {{}}"
                    );
        return manager;
    }

    private static IPuzzleManager CreatePuzzleManager()
    {
        var manager = Substitute.For<IPuzzleManager>();
        foreach (var y in AoCLogic.Years())
            foreach (var d in Enumerable.Range(1, 25))
                manager.GetPuzzleResult(y, d, Arg.Any<Action<int, Result>>()).Returns(
                    Task.FromResult(new PuzzleResultStatus(
                        new Puzzle(y, d, string.Empty, string.Empty, string.Empty, Answer.Empty, Status.Unlocked),
                        DayResult.NotImplemented(y, d), false))
                    );

        return manager;
    }

}
