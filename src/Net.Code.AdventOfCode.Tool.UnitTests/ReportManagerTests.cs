﻿using Net.Code.AdventOfCode.Tool.Core;
using Net.Code.AdventOfCode.Tool.Logic;

using NSubstitute;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;

using static Net.Code.AdventOfCode.Tool.UnitTests.TestClock;

namespace Net.Code.AdventOfCode.Tool.UnitTests;

public class ReportManagerTests 
{
    [Fact]
    public async Task GetPuzzleReportTest()
    {
        SetClock(2017,1,1,0,0,0);
        var client = Substitute.For<IAoCClient>();
        var manager = Substitute.For<IPuzzleManager>();

        manager.GetPuzzleResult(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Action<int,Result>>())
            .Returns(callInfo => new PuzzleResultStatus(
                new Puzzle(callInfo.ArgAt<int>(0), callInfo.ArgAt<int>(1), "", "", "", Answer.Empty, Status.Unlocked),
                DayResult.NotImplemented(callInfo.ArgAt<int>(0), callInfo.ArgAt<int>(1)),
                false
                )
            );

        var rm = new ReportManager(client, manager);

        var report = await rm.GetPuzzleReport(null, null).ToListAsync();

        Assert.Equal(50, report.Count);
    }

    [Fact]
    public async Task GetMemberStatsTest()
    {
        SetClock(2017, 1, 1, 0, 0, 0);
        var client = Substitute.For<IAoCClient>();
        var manager = Substitute.For<IPuzzleManager>();

        client.GetMemberAsync(Arg.Any<int>(), true)
            .Returns(callInfo => Task.FromResult(new Member(1, "", 0, 0, 0, AoCLogic.Clock.GetCurrentInstant(), new Dictionary<int,DailyStars>()))
            );

        var rm = new ReportManager(client, manager);

        var report = await rm.GetMemberStats().ToListAsync();

        Assert.Equal(3, report.Count);

    }
}