enum ReadState
{
    Begin,
    Checksum,
    State,
    IfValueIs,
    Write,
    Move,
    Continue
}
public class Specs
{
    [Fact]
    public void ReadSampleInput()
    {
        var input = @"Begin in state A.
Perform a diagnostic checksum after 6 steps.

In state A:
  If the current value is 0:
    - Write the value 1.
    - Move one slot to the right.
    - Continue with state B.
  If the current value is 1:
    - Write the value 0.
    - Move one slot to the left.
    - Continue with state B.

In state B:
  If the current value is 0:
    - Write the value 1.
    - Move one slot to the left.
    - Continue with state A.
  If the current value is 1:
    - Write the value 1.
    - Move one slot to the right.
    - Continue with state A.";


        string expected = "A01+B\r\n" +
                          "A10-B\r\n" +
                          "B01-A\r\n" +
                          "B11+A";

        var result = input.EncodeToSomethingSimpler();

        Assert.Equal(expected, result.code);
        Assert.Equal('A', result.beginState);
        Assert.Equal(6, result.checksum);

    }

    [Fact]
    public void Test()
    {
        var code = "A01+B\r\n" +
                   "A10-B\r\n" +
                   "B01-A\r\n" +
                   "B11+A";

        var actual = ('A', 6, code).CalculateChecksum();

        Assert.Equal(3, actual);
    }

}