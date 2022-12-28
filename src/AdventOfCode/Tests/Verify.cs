using Net.Code.AdventOfCode.Toolkit;

using System.Runtime.CompilerServices;

namespace AdventOfCode.Tests
{

    public class Verify
    {
        ITestOutputHelper output;

        public Verify(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [MemberData(nameof(Verify.GetData))]
        public async Task VerifyPuzzle(int year, int day)
        {
            Environment.CurrentDirectory = GetProjectPath();
            var assembly = Assembly.GetExecutingAssembly();
            var engine = await AoC.GetEngine(assembly, s => output.WriteLine(s));
            var (result, message) = await engine.GetResultAsync(year, day);
            Assert.True(result, message);
        }

        private string GetProjectPath([CallerFilePath] string? path = null)
        {
            return new DirectoryInfo(Path.GetDirectoryName(path)!)!.Parent!.FullName;
        }

        public static IEnumerable<object[]> GetData()
        {
            var now = DateTime.Now;
            for (int year = 2015; year <= now.Year; year++)
                for (int day = 1; year < now.Year && day <= 25 ||
                                  now.Month == 12 && day <= Min(25, now.Day); day++)
                    yield return new object[] { year, day };
        }
    }
}
