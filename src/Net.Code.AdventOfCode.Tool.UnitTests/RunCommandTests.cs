using Net.Code.AdventOfCode.Tool.Core;

using NSubstitute;

using System;
using System.Linq;
using System.Threading.Tasks;

using Xunit;

namespace Net.Code.AdventOfCode.Tool.UnitTests;

public class RunCommandTests
{
    [Fact]
    public async Task Run_WhenCalledWithYearDay_RunsPuzzleForYearDay()
    {
        TestClock.SetClock(2021, 12, 26, 0, 0, 0);
        var manager = Substitute.For<IAoCRunner>();
        var run = new Commands.Run(manager);
        await run.ExecuteAsync(default, new Commands.Run.Settings { year = 2021, typeName = String.Empty, day = 1 });
        await manager.Received().Run(string.Empty, 2021, 1, Arg.Any<Action<int, Result>>());
    }

    [Fact]
    public async Task Run_WhenCalledWithYearOutsideAdvent_RunsPuzzlesForThatYear()
    {
        TestClock.SetClock(2021, 12, 26, 0, 0, 0);
        var manager = Substitute.For<IAoCRunner>();
        var run = new Commands.Run(manager);
        await run.ExecuteAsync(default!, new Commands.Run.Settings { year = 2021 });
        foreach (var d in Enumerable.Range(1, 25))
            await manager.Received().Run(Arg.Any<string>(), 2021, d, Arg.Any<Action<int, Result>>());
    }

    [Fact]
    public async Task Run_WhenCalledWithYearDuringAdvent_RunsPuzzlesForThatYear()
    {
        TestClock.SetClock(2021, 12, 17, 0, 0, 0);
        var manager = Substitute.For<IAoCRunner>();
        var run = new Commands.Run(manager);
        await run.ExecuteAsync(default!, new Commands.Run.Settings { year = 2021 });
        foreach (var d in Enumerable.Range(1, 17))
            await manager.Received().Run(Arg.Any<string>(), 2021, d, Arg.Any<Action<int, Result>>());
    }

    [Fact]
    public async Task Run_WhenCalledWithoutArgumentsDuringAdvent_RunsPuzzleForThatDay()
    {
        TestClock.SetClock(2021, 12, 17, 0, 0, 0);
        var manager = Substitute.For<IAoCRunner>();
        var run = new Commands.Run(manager);
        await run.ExecuteAsync(default!, new Commands.Run.Settings { });
        await manager.Received(1).Run(Arg.Any<string>(), 2021, 17, Arg.Any<Action<int, Result>>());
        await manager.Received(1).Run(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Action<int, Result>>());
    }

    [Fact]
    public async Task Run_WhenCalledWithoutArgumentsBeforeAdvent_RunsAllPuzzles()
    {
        TestClock.SetClock(2018, 1, 1, 0, 0, 0);
        var manager = Substitute.For<IAoCRunner>();
        var run = new Commands.Run(manager);
        await run.ExecuteAsync(default!, new Commands.Run.Settings { });
        foreach (var y in new[] { 2015, 2016, 2017 })
            foreach (var d in Enumerable.Range(1, 25))
                await manager.Received(1).Run(Arg.Any<string>(), y, d, Arg.Any<Action<int, Result>>());
        await manager.Received(75).Run(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Action<int, Result>>());
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
