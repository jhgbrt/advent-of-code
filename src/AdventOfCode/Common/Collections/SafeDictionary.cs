using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Common.Collections;
#nullable disable
public class SafeDictionary<TKey, TValue> : IDictionary<TKey, TValue> where TKey : notnull
{
    Dictionary<TKey, TValue> _inner = [];

    public TValue this[TKey key]
    {
        get => _inner.TryGetValue(key, out var value) ? value : default;
        set => _inner[key] = value;
    }

    public ICollection<TKey> Keys => _inner.Keys;

    public ICollection<TValue> Values => _inner.Values;

    public int Count => _inner.Count;

    public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)_inner).IsReadOnly;

    public void Add(TKey key, TValue value) => _inner.Add(key, value);

    public void Add(KeyValuePair<TKey, TValue> item) => _inner.Add(item.Key, item.Value);

    public void Clear() => _inner.Clear();

    public bool Contains(KeyValuePair<TKey, TValue> item) => _inner.Contains(item);

    public bool ContainsKey(TKey key) => _inner.ContainsKey(key);

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((ICollection<KeyValuePair<TKey, TValue>>)_inner).CopyTo(array, arrayIndex);

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => ((IEnumerable<KeyValuePair<TKey, TValue>>)_inner).GetEnumerator();

    public bool Remove(TKey key) => _inner.Remove(key);

    public bool Remove(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)_inner).Remove(item);

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) => _inner.TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator() => _inner.GetEnumerator();
}