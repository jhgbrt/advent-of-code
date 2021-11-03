using System.Linq;

class Captcha
{
    public static int Calculate(string input, int lookahead)
        => input.Select((c, i) => (character: c, index: i))
            .Where(_ => _.character == input[(_.index + lookahead) % input.Length])
            .Select(_ => _.character - '0')
            .Sum();
}