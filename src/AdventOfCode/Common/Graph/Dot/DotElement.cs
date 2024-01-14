using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;

namespace Net.Code.Graph.Dot;

public class AttributeCollection : IEnumerable<KeyValuePair<string, DotAttribute>>
{
    public Dictionary<string, DotAttribute> Attributes { get; } = [];

    public DotAttribute? this[string key]
    {
        get { return Attributes.ContainsKey(key) ? Attributes[key] : default; }
        set { if (value is null) Remove(key); else Attributes[key] = value; }
    }

    public bool TryGetValue(string key, out DotAttribute? value)
    {
        return Attributes.TryGetValue(key, out value);
    }
    public bool Remove(string key)
    {
        return Attributes.Remove(key);
    }

    public IEnumerator<KeyValuePair<string, DotAttribute>> GetEnumerator()
    {
        return Attributes.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Attributes.GetEnumerator();
    }

    public int Count => Attributes.Count;
}

public abstract class DotElement
{
    public AttributeCollection Attributes { get; } = new();

    // Common attributes
    public DotLabelAttribute? Label
    {
        get => GetAttribute<DotLabelAttribute>("label");
        set => SetAttribute("label", value);
    }

    public DotColorAttribute? FontColor
    {
        get => GetAttribute<DotColorAttribute>("fontcolor");
        set => SetAttribute("fontcolor", value);
    }

    public DotAttribute? GetAttribute(string name)
    {
        return Attributes[name];
    }

    public T? GetAttribute<T>(string name) where T : DotAttribute
    {
        if (Attributes.TryGetValue(name, out var result))
            return (T?)result;
        return default;
    }

    public void SetAttribute(string name, DotAttribute? value)
    {
        Attributes[name] = value;
    }
}