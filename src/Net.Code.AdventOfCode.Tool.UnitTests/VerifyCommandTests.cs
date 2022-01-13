using Net.Code.AdventOfCode.Tool.Core;

using NSubstitute;

using System;
using System.Linq;
using System.Threading.Tasks;

using Xunit;

namespace Net.Code.AdventOfCode.Tool.UnitTests;

public class VerifyCommandTests
{
    private static IPuzzleManager CreatePuzzleManager()
    {
        var manager = Substitute.For<IPuzzleManager>();
        foreach (var y in AoCLogic.Years())
            foreach (var d in Enumerable.Range(1, 25))
                manager.GetPuzzleResult(2021, d, Arg.Any<Action<int, Result>>()).Returns(
                    Task.FromResult(new PuzzleResultStatus(
                        new Puzzle(y, d, String.Empty, string.Empty, string.Empty, Answer.Empty, Status.Unlocked),
                        DayResult.NotImplemented(2021, d), false))
                    );
        return manager;
    }
    [Fact]
    public async Task Verify_WhenCalledWithYearDay_RunsPuzzleForYearDay()
    {
        TestClock.SetClock(2021, 12, 26, 0, 0, 0);
        IPuzzleManager manager = CreatePuzzleManager();
        var run = new Commands.Verify(manager);
        await run.ExecuteAsync(default, new Commands.Verify.Settings { year = 2021, day = 1 });
        await manager.Received().GetPuzzleResult(2021, 1, Arg.Any<Action<int, Result>>());
    }

    [Fact]
    public async Task Verify_WhenCalledWithYearOutsideAdvent_RunsPuzzlesForThatYear()
    {
        TestClock.SetClock(2021, 12, 26, 0, 0, 0);
        IPuzzleManager manager = CreatePuzzleManager();
        var run = new Commands.Verify(manager);
        await run.ExecuteAsync(default!, new Commands.Verify.Settings { year = 2021 });
        foreach (var d in Enumerable.Range(1, 25))
            await manager.Received().GetPuzzleResult(2021, d, Arg.Any<Action<int, Result>>());
    }


    [Fact]
    public async Task Verify_WhenCalledWithYearDuringAdvent_RunsPuzzlesForThatYear()
    {
        TestClock.SetClock(2021, 12, 17, 0, 0, 0);
        IPuzzleManager manager = CreatePuzzleManager();
        var run = new Commands.Verify(manager);
        await run.ExecuteAsync(default!, new Commands.Verify.Settings { year = 2021 });
        foreach (var d in Enumerable.Range(1, 17))
            await manager.Received().GetPuzzleResult(2021, d, Arg.Any<Action<int, Result>>());
    }

    [Fact]
    public async Task Verify_WhenCalledWithoutArgumentsDuringAdvent_RunsPuzzleForThatDay()
    {
        TestClock.SetClock(2021, 12, 17, 0, 0, 0);
        IPuzzleManager manager = CreatePuzzleManager();
        var run = new Commands.Verify(manager);
        await run.ExecuteAsync(default!, new Commands.Verify.Settings { });
        await manager.Received(1).GetPuzzleResult(2021, 17, Arg.Any<Action<int, Result>>());
        await manager.Received(1).GetPuzzleResult(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Action<int, Result>>());
    }

    [Fact]
    public async Task Run_WhenCalledWithoutArgumentsInDecemberAfterAdvent_RunsAllPuzzlesForCurrentYear()
    {
        TestClock.SetClock(2018, 12, 26, 0, 0, 0);
        var manager = Substitute.For<IAoCRunner>();
        var run = new Commands.Run(manager);
        await run.ExecuteAsync(default!, new Commands.Run.Settings { });
        foreach (var y in new[] { 2018 })
            foreach (var d in Enumerable.Range(1, 25))
                await manager.Received(1).Run(Arg.Any<string>(), y, d, Arg.Any<Action<int, Result>>());
        await manager.Received(25).Run(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Action<int, Result>>());
    }
}
