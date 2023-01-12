using System.Globalization;

namespace AdventOfCode.Tests;
record struct MyRecord1(string s1, string s2);
record struct MyRecord2(string s1, int i1);
record struct MyRecord3(string s1, decimal d1);
record struct MyRecord4(DateTime d1);
record struct MyRecord5(string type, string somevalue);
public class RegexHelperTests
{
    ITestOutputHelper _output;

    public RegexHelperTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void CanParseRecordWithStringProperties()
    {
        var r = new Regex(@"--(?<s1>\w+)---(?<s2>\w+)");
        var result = r.As<MyRecord1>("--abc---def");
        Assert.Equal(new MyRecord1("abc", "def"), result);
    }
    [Fact]
    public void CanParseRecordWithIntProperties()
    {
        var r = new Regex(@"--(?<s1>\w+)---(?<i1>\d+)--");
        var result = r.As<MyRecord2>("--abc---12--");
        Assert.Equal(new MyRecord2("abc", 12), result);
    }

    [Fact]
    public void CanParseRecordWithDecimalProperties()
    {
        var r = new Regex(@"--(?<s1>\w+)---(?<d1>[\.,\d]+)--");
        var result = r.As<MyRecord3>("--abc---12,23--", provider: CultureInfo.GetCultureInfoByIetfLanguageTag("nl"));
        _output.WriteLine(result.ToString());
        Assert.Equal(new MyRecord3("abc", 12.23m), result);
    }

    [Fact]
    public void CanParseRecordWithDecimalPropertiesAndCulture()
    {
        var r = new Regex(@"--(?<s1>\w+)---(?<d1>[\.,\d]+)--");
        var result = r.As<MyRecord3>("--abc---12.23--", provider: CultureInfo.InvariantCulture);
        _output.WriteLine(result.ToString());
        Assert.Equal(new MyRecord3("abc", 12.23m), result);
    }
    [Fact]
    public void CanParseRecordWithDateProperty()
    {
        var r = new Regex(@"--(?<d1>.+)--");
        var result = r.As<MyRecord4>("--2021-12-31--");
        _output.WriteLine(result.ToString());
        Assert.Equal(new MyRecord4(new DateTime(2021, 12, 31)), result);
    }

    [Fact]
    public void CanParseRecordWithDatePropertyAndSpecificCulture()
    {
        var r = new Regex(@"--(?<d1>.+)--");
        var result = r.As<MyRecord4>("--31/12/2021--", provider: CultureInfo.GetCultureInfoByIetfLanguageTag("nl"));
        _output.WriteLine(result.ToString());
        Assert.Equal(new MyRecord4(new DateTime(2021, 12, 31)), result);
    }

    [Fact]
    public void CanParseRecordWithAdditionalParameter()
    {
        var r = new Regex(@"--(?<somevalue>.+)--");
        var result = r.As<MyRecord5>("--somevalue--",
            new { type = "type" },
            provider: CultureInfo.GetCultureInfoByIetfLanguageTag("nl"));
        _output.WriteLine(result.ToString());
        Assert.Equal(new MyRecord5("type", "somevalue"), result);
    }
}

public class FormattingTests
{
    [Fact]
    public void FormatArray_Default()
    {
        var result = string.Format(new CsvLineFormatInfo(), "{0}", new[] { 'a', 'b', 'c' });
        Assert.Equal("a,b,c", result);
    }

    [Fact]
    public void FormatArray_Delimiter()
    {
        var result = string.Format(new CsvLineFormatInfo(), "{0:;}", new[] { 'a', 'b', 'c' });
        Assert.Equal("a;b;c", result);
    }

    [Fact]
    public void ConvertToArray()
    {
        var result = MyConvert.ChangeType("a,b,c", typeof(char[]), new CsvLineFormatInfo());
        Assert.Equal(new[] { 'a', 'b', 'c' }, result);
    }
    [Fact]
    public void ConvertToEnumerable()
    {
        var result = MyConvert.ChangeType("1,2,3", typeof(IEnumerable<int>), new CsvLineFormatInfo());
        Assert.Equal(new[] { 1, 2, 3 }, result);
    }
}
