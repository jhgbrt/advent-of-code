using Net.Code.AdventOfCode.Tool.Commands;

using NSubstitute;
using NSubstitute.Extensions;

using Spectre.Console.Cli;

using System;
using System.Threading.Tasks;

using Xunit;

namespace Net.Code.AdventOfCode.Tool.UnitTests;

public class SinglePuzzlesCommandTest
{
    [Fact]
    public async Task NoYearNoDay_OutsideAdvent_Throws()
    {
        TestClock.SetClock(2017, 1, 1, 0, 0, 0);
        var sut = Substitute.ForPartsOf<SinglePuzzleCommand<AoCSettings>>();
        var context = new CommandContext(Substitute.For<IRemainingArguments>(), "name", default);
        var options = new AoCSettings();
        await sut.Configure().ExecuteAsync(Arg.Any<int>(), Arg.Any<int>(), options);
        await Assert.ThrowsAsync<Exception>(() => sut.ExecuteAsync(context, options));
    }
    [Fact]
    public async Task NoYearNoDay_OutsideAdventInDecember_Throws()
    {
        TestClock.SetClock(2016, 12, 26, 0, 0, 0);
        var sut = Substitute.ForPartsOf<SinglePuzzleCommand<AoCSettings>>();
        var context = new CommandContext(Substitute.For<IRemainingArguments>(), "name", default);
        var options = new AoCSettings();
        await sut.Configure().ExecuteAsync(Arg.Any<int>(), Arg.Any<int>(), options);
        await Assert.ThrowsAsync<Exception>(() => sut.ExecuteAsync(context, options));
    }
    [Fact]
    public async Task NoYearNoDay_DuringAdvent_Throws()
    {
        TestClock.SetClock(2017, 12, 20, 0, 0, 0);
        var sut = Substitute.ForPartsOf<SinglePuzzleCommand<AoCSettings>>();
        var context = new CommandContext(Substitute.For<IRemainingArguments>(), "name", default);
        var options = new AoCSettings();
        await sut.Configure().ExecuteAsync(Arg.Any<int>(), Arg.Any<int>(), options);
        await Assert.ThrowsAsync<Exception>(() => sut.ExecuteAsync(context, options));
    }
    [Fact]
    public async Task YearNoDay_OutsideAdvent_Throws()
    {
        TestClock.SetClock(2017, 1, 1, 0, 0, 0);
        var sut = Substitute.ForPartsOf<SinglePuzzleCommand<AoCSettings>>();
        var context = new CommandContext(Substitute.For<IRemainingArguments>(), "name", default);
        var options = new AoCSettings { year = 2016 };
        await sut.Configure().ExecuteAsync(Arg.Any<int>(), Arg.Any<int>(), options);
        await Assert.ThrowsAsync<Exception>(() => sut.ExecuteAsync(context, options));
    }
    [Fact]
    public async Task YearNoDay_OutsideAdventInDecember_Throws()
    {
        TestClock.SetClock(2016, 12, 26, 0, 0, 0);
        var sut = Substitute.ForPartsOf<SinglePuzzleCommand<AoCSettings>>();
        var context = new CommandContext(Substitute.For<IRemainingArguments>(), "name", default);
        var options = new AoCSettings { year = 2016 };
        await sut.Configure().ExecuteAsync(Arg.Any<int>(), Arg.Any<int>(), options);
        await Assert.ThrowsAsync<Exception>(() => sut.ExecuteAsync(context, options));
    }
    [Fact]
    public async Task YearNoDay_DuringAdvent_ForPastYear_Throws()
    {
        TestClock.SetClock(2017, 12, 20, 0, 0, 0);
        var sut = Substitute.ForPartsOf<SinglePuzzleCommand<AoCSettings>>();
        var context = new CommandContext(Substitute.For<IRemainingArguments>(), "name", default);
        var options = new AoCSettings { year = 2016 };
        await sut.Configure().ExecuteAsync(Arg.Any<int>(), Arg.Any<int>(), options);
        await Assert.ThrowsAsync<Exception>(() => sut.ExecuteAsync(context, options));
    }
    [Fact]
    public async Task YearNoDay_DuringAdvent_ForCurrentYear_Throws()
    {
        TestClock.SetClock(2017, 12, 20, 0, 0, 0);
        var sut = Substitute.ForPartsOf<SinglePuzzleCommand<AoCSettings>>();
        var context = new CommandContext(Substitute.For<IRemainingArguments>(), "name", default);
        var options = new AoCSettings { year = 2017 };
        await sut.Configure().ExecuteAsync(Arg.Any<int>(), Arg.Any<int>(), options);
        await Assert.ThrowsAsync<Exception>(() => sut.ExecuteAsync(context, options));
    }
    [Fact]
    public async Task NoYearDay_DuringAdvent_Throws()
    {
        TestClock.SetClock(2017, 12, 20, 0, 0, 0);
        var sut = Substitute.ForPartsOf<SinglePuzzleCommand<AoCSettings>>();
        var context = new CommandContext(Substitute.For<IRemainingArguments>(), "name", default);
        var options = new AoCSettings { day = 15 };
        await sut.Configure().ExecuteAsync(Arg.Any<int>(), Arg.Any<int>(), options);
        await Assert.ThrowsAsync<Exception>(() => sut.ExecuteAsync(context, options));
    }
    [Fact]
    public async Task NoYearDay_OutsideAdvent_InDecember_Throws()
    {
        TestClock.SetClock(2017, 12, 26, 0, 0, 0);
        var sut = Substitute.ForPartsOf<SinglePuzzleCommand<AoCSettings>>();
        var context = new CommandContext(Substitute.For<IRemainingArguments>(), "name", default);
        var options = new AoCSettings { day = 15 };
        await sut.Configure().ExecuteAsync(Arg.Any<int>(), Arg.Any<int>(), options);
        await Assert.ThrowsAsync<Exception>(() => sut.ExecuteAsync(context, options));
    }
    [Fact]
    public async Task NoYearDay_OutsideAdvent_Throws()
    {
        TestClock.SetClock(2017, 1, 1, 0, 0, 0);
        var sut = Substitute.ForPartsOf<SinglePuzzleCommand<AoCSettings>>();
        var context = new CommandContext(Substitute.For<IRemainingArguments>(), "name", default);
        var options = new AoCSettings { day = 15 };
        await sut.Configure().ExecuteAsync(Arg.Any<int>(), Arg.Any<int>(), options);
        await Assert.ThrowsAsync<Exception>(() => sut.ExecuteAsync(context, options));
    }
    [Fact]
    public async Task YearDay_OutsideAdvent_RunsSinglePuzzle()
    {
        TestClock.SetClock(2017, 1, 1, 0, 0, 0);
        var sut = Substitute.ForPartsOf<SinglePuzzleCommand<AoCSettings>>();
        var context = new CommandContext(Substitute.For<IRemainingArguments>(), "name", default);
        var options = new AoCSettings { year = 2016, day = 23 };
        await sut.Configure().ExecuteAsync(Arg.Any<int>(), Arg.Any<int>(), options);
        await sut.ExecuteAsync(context, options);
        await sut.Received(1).ExecuteAsync(2016, 23, options);
    }
    [Fact]
    public async Task YearDay_OutsideAdventInDecember_RunsSinglePuzzle()
    {
        TestClock.SetClock(2016, 12, 26, 0, 0, 0);
        var sut = Substitute.ForPartsOf<ManyPuzzlesCommand<AoCSettings>>();
        var context = new CommandContext(Substitute.For<IRemainingArguments>(), "name", default);
        var options = new AoCSettings { year = 2016, day = 23 };
        await sut.Configure().ExecuteAsync(Arg.Any<int>(), Arg.Any<int>(), options);
        await sut.ExecuteAsync(context, options);
        await sut.Received(1).ExecuteAsync(2016, 23, options);
    }
    [Fact]
    public async Task YearDay_DuringAdvent_RunsPuzzleForCurrentDay()
    {
        TestClock.SetClock(2017, 12, 20, 0, 0, 0);
        var sut = Substitute.ForPartsOf<ManyPuzzlesCommand<AoCSettings>>();
        var context = new CommandContext(Substitute.For<IRemainingArguments>(), "name", default);
        var options = new AoCSettings { year = 2017, day = 19 };
        await sut.Configure().ExecuteAsync(Arg.Any<int>(), Arg.Any<int>(), options);
        await sut.ExecuteAsync(context, options);
        await sut.Received(1).ExecuteAsync(2017, 19, options);
    }
    [Fact]
    public async Task YearDay_DuringAdvent_FuturePuzzle()
    {
        TestClock.SetClock(2017, 12, 20, 0, 0, 0);
        var sut = Substitute.ForPartsOf<ManyPuzzlesCommand<AoCSettings>>();
        var context = new CommandContext(Substitute.For<IRemainingArguments>(), "name", default);
        var options = new AoCSettings { year = 2017, day = 23 };
        await sut.Configure().ExecuteAsync(Arg.Any<int>(), Arg.Any<int>(), options);
        await sut.ExecuteAsync(context, options);
        await sut.DidNotReceive().ExecuteAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<AoCSettings>());
    }
}
