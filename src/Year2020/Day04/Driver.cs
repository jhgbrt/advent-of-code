using Microsoft.CodeAnalysis.CSharp;

namespace AdventOfCode.Year2020.Day04;

static class Driver
{
    internal static long Part1(string input) => Parse(input).Count(RequiredFieldsPresent);
    internal static long Part2(string input) => Parse(input).Where(RequiredFieldsPresent).Count(IsValid);
    internal static IEnumerable<IReadOnlyDictionary<string, string>> Parse(string path, Action<string>? log = null)
        => Parse(new StringReader(Read.Text(typeof(AoCImpl), path).Replace("\r\n", "\n")), log);

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

    static string ToCSharpLiteral(this char c) => SymbolDisplay.FormatLiteral(c, false);

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