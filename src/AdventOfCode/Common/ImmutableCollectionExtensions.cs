namespace AdventOfCode;

static class ImmutableCollectionExtensions
{
    public static ValueList<T> WithValueSemantics<T>(this IEnumerable<T> list) => new ValueList<T>(list);
}