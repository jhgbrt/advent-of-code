using Microsoft.CodeAnalysis.CSharp;

namespace AdventOfCode.Year2020.Day04;

public class AoC202004
{
    public object Part1() => Parse(Read.InputText()).Count(Ex.RequiredFieldsPresent);
    public object Part2() => Parse(Read.InputText()).Where(Ex.RequiredFieldsPresent).Count(Ex.IsValid);
    internal static IEnumerable<IReadOnlyDictionary<string, string>> Parse(string input, Action<string>? log = null)
        => Parse(new StringReader(input.Replace("\r\n", "\n")), log);

    internal static IEnumerable<IReadOnlyDictionary<string, string>> Parse(TextReader reader, Action<string>? log = null)
    {
        if (log == null) log = s => { };
        var cb = new CredentialBuilder();
        TransitionFunc Next = OnKey;
        while (reader.Peek() >= 0)
        {
            char c = (char)reader.Read();
            log(Next.Method.Name);
            log(c.ToCSharpLiteral());
            (cb, Next) = Next(cb, c);
            log(cb.ToString());
            if (cb.IsComplete)
                yield return cb.ToCredential();

        }
        yield return cb.RecordKeyValue().Finalize().ToCredential();
    }

    delegate (CredentialBuilder, TransitionFunc) TransitionFunc(CredentialBuilder cb, char c);

    static (CredentialBuilder, TransitionFunc) OnKey(CredentialBuilder cb, char c) => c switch
    {
        ':' => (cb, OnValue),
        _ when char.IsLetter(c) => (cb.AppendToKey(c), OnKey),
        _ => throw new($"unexpected character while parsing key: {c}")
    };

    static (CredentialBuilder, TransitionFunc) OnValue(CredentialBuilder cb, char c) => c switch
    {
        ' ' => (cb.RecordKeyValue(), OnKey),
        '\n' => (cb.RecordKeyValue(), OnFirstNewLine),
        _ => (cb.AppendToValue(c), OnValue)
    };

    static (CredentialBuilder, TransitionFunc) OnFirstNewLine(CredentialBuilder cb, char c) => c switch
    {
        '\n' => (cb.Finalize(), OnAdditionalNewLine), // second newline
        _ => (cb.AppendToKey(c), OnKey)
    };

    static (CredentialBuilder, TransitionFunc) OnAdditionalNewLine(CredentialBuilder cb, char c) => c switch
    {
        '\r' or '\n' => (cb, OnAdditionalNewLine),
        _ => (cb.AppendToKey(c), OnKey)
    };


}
static class Ex
{
    internal static string ToCSharpLiteral(this char c) => SymbolDisplay.FormatLiteral(c, false);
    static readonly string[] ValidKeys = new[] { "byr", "ecl", "eyr", "hcl", "hgt", "iyr", "pid" };

    internal static bool RequiredFieldsPresent(this IReadOnlyDictionary<string, string> credential) => (
        from key in credential.Keys
        join vkey in ValidKeys on key equals vkey
        select key
        ).Count() == ValidKeys.Length;

    static readonly Regex ColorRegex = new("^#[0-9a-f]{6}$", RegexOptions.Compiled);

    internal static bool IsValid(this IReadOnlyDictionary<string, string> credential)
        => credential.All(kv => IsValid(kv.Key, kv.Value));

    internal static bool IsValid(string key, string value) => key switch
    {
        "byr" => int.TryParse(value, out var y) && y >= 1920 && y <= 2002,
        "iyr" => int.TryParse(value, out var y) && y >= 2010 && y <= 2020,
        "eyr" => int.TryParse(value, out var y) && y >= 2020 && y <= 2030,
        "hgt" => Amount.TryParse(value, out var hgt) && hgt switch
        {
            { Unit: "cm", Value: >= 150 and <= 193 } => true,
            { Unit: "in", Value: >= 59 and <= 76 } => true,
            _ => false
        },
        "hcl" => ColorRegex.IsMatch(value),
        "ecl" => value switch
        {
            "amb" or "blu" or "brn" or "gry" or "grn" or "hzl" or "oth" => true,
            _ => false
        },
        "pid" => value.Length == 9 && value.All(char.IsDigit),
        _ => true
    };
}
class CredentialBuilder
{
    StringBuilder key = new();
    StringBuilder value = new();
    Dictionary<string, string> dictionary = new();

    internal bool IsComplete { get; private set; }

    internal CredentialBuilder AppendToKey(char c)
    {
        key.Append(c);
        return this;
    }
    internal CredentialBuilder AppendToValue(char c)
    {
        value.Append(c);
        return this;
    }
    internal CredentialBuilder RecordKeyValue()
    {
        dictionary[key.ToString()] = value.ToString();
        key.Clear();
        value.Clear();
        return this;
    }
    internal CredentialBuilder Finalize()
    {
        IsComplete = true;
        return this;
    }
    internal IReadOnlyDictionary<string, string> ToCredential()
    {
        var result = dictionary;
        IsComplete = false;
        dictionary = new();
        return result;
    }

    public override string ToString()
    {
        return $"{string.Join(";", dictionary.Select(d => $"{d.Key}:{d.Value}"))} + {key}:{value} (complete: {IsComplete})";
    }
}

record Amount(int Value, string Unit)
{
    static readonly Regex AmountRegex = new(@"^(?<Value>\d+)(?<Unit>[a-z]+)$");
    public static bool TryParse(string input, out Amount? result)
    {
        var match = AmountRegex.Match(input);
        if (!match.Success)
        {
            result = default;
            return false;
        }
        result = new(int.Parse(match.Groups["Value"].Value), match.Groups["Unit"].Value);
        return true;
    }
}


