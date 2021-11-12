namespace AdventOfCode
{
    public class Specs
    {
        string[] input = new[]
        {
                "[1518-11-01 00:00] Guard #10 begins shift",
                "[1518-11-01 00:05] falls asleep",
                "[1518-11-01 00:25] wakes up",
                "[1518-11-01 00:30] falls asleep",
                "[1518-11-01 00:55] wakes up",
                "[1518-11-01 23:58] Guard #99 begins shift",
                "[1518-11-02 00:40] falls asleep",
                "[1518-11-02 00:50] wakes up",
                "[1518-11-03 00:05] Guard #10 begins shift",
                "[1518-11-03 00:24] falls asleep",
                "[1518-11-03 00:29] wakes up",
                "[1518-11-04 00:02] Guard #99 begins shift",
                "[1518-11-04 00:36] falls asleep",
                "[1518-11-04 00:46] wakes up",
                "[1518-11-05 00:03] Guard #99 begins shift",
                "[1518-11-05 00:45] falls asleep",
                "[1518-11-05 00:55] wakes up"
            };

        [Fact]
        public void GuardAction_BeginShift()
        {
            var input = "[1518-11-01 00:00] Guard #10 begins shift";

            var action = Parser.ToGuardActions(new[] { input }).Single();

            Assert.Equal(GuardAction.Type.StartShift, action.ActionType);
            Assert.Equal(new DateTime(1518, 11, 1), action.TimeStamp);

        }
        [Fact]
        public void GuardAction_FellAsleep()
        {
            var input = "[1518-11-01 00:05] falls asleep";

            var action = Parser.ToGuardActions(new[] { input }).Single();

            Assert.Equal(GuardAction.Type.FellAsleep, action.ActionType);
            Assert.Equal(new DateTime(1518, 11, 1, 0, 5, 0), action.TimeStamp);

        }

        [Fact]
        public void GuardAction_WakesUp()
        {
            var input = "[1518-11-01 00:05] wakes up";

            var action = Parser.ToGuardActions(new[] { input }).Single();

            Assert.Equal(GuardAction.Type.WakesUp, action.ActionType);
            Assert.Equal(new DateTime(1518, 11, 1, 0, 5, 0), action.TimeStamp);

        }

        [Fact]
        public void CountMinutesAsleep()
        {
            var input = new[]
            {
                "[1518-11-01 00:00] Guard #10 begins shift",
                "[1518-11-01 00:05] falls asleep",
                "[1518-11-01 00:25] wakes up",
                "[1518-11-01 00:30] falls asleep",
                "[1518-11-01 00:55] wakes up"
            };

            var guards = (from guardaction in Parser.ToGuardActions(input)
                          orderby guardaction.TimeStamp
                          group guardaction by guardaction.ID)
                          .Single();
            var result = guards.CountMinutesAsleep();

            Assert.Equal(45, result);
        }

        [Fact]
        public void MostSleepingMinute()
        {
            var guards = (from guardaction in Parser.ToGuardActions(input)
                          orderby guardaction.TimeStamp
                          group guardaction by guardaction.ID)
                          .Where(g => g.Key == 10)
                          .Single();

            var result = guards.GetMostSleepingMinute();

            Assert.Equal(24, result);
        }

        [Fact]
        public void TestPart1()
        {
            var result = AoC.Part1(input);
            Assert.Equal(24 * 10, result);
        }

        [Fact]
        public void TestPart2()
        {
            var result = AoC.Part2(input);
            Assert.Equal(99*45, result);
        }

        [Fact]
        public void MinuteMostAsleep()
        {

            var guard = (from guardaction in Parser.ToGuardActions(input)
                          orderby guardaction.TimeStamp
                          group guardaction by guardaction.ID)
                          .Where(g => g.Key == 99)
                          .Single();

            var result = guard.GetMostSleepingMinute();

            Assert.Equal(45, result);
        }
    }
}
