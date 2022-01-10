namespace AdventOfCode.Year2020.Day04;

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